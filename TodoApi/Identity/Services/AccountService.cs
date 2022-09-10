using Application.DTOs.Customer;
using Application.DTOs.Gemini;
using Application.Features.Gemini.CustomerInfo.Command.CreateInfo;
using Application.Features.Gemini.CustomerKin.Command.CreateKin;
using Application.Features.Gemini.Employers.Command.CreateEmployer;
using Application.Features.Gemini.Identification.Command.CreateIdentification;
using MediatR;
using Shared.Services;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;
        private readonly IdentityContext _context;
        private readonly IUserProfileRepositoryAsync _userProfile;
     
        private readonly IMediator _mediator;
        private readonly GeminiApiSettings _geminiApiSettings;
        private readonly ApplicationUrl _applicationUrlSettings;

        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWTSettings> jwtSettings,
            IDateTimeService dateTimeService,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IdentityContext context,
            IUserProfileRepositoryAsync userProfile,
             IOptionsSnapshot<ApplicationUrl> applicationUrlSettings, IMediator mediator, GeminiApiSettings geminiApiSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _dateTimeService = dateTimeService;
            _signInManager = signInManager;
            this._emailService = emailService;
            _context = context;
            _userProfile = userProfile;
          
            _mediator = mediator;
            _geminiApiSettings = geminiApiSettings;
            _applicationUrlSettings = applicationUrlSettings.Value;
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.Email);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.Email}' is already taken.");
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    //var otp = GenerateOTP();
                    var url = await GenerateVerificationEmail(user);

                    await _userManager.AddToRoleAsync(user, ((Roles)request.RoleId).ToString());

                    var userProfile = new UserProfile
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        OtherName = request.OtherName,
                        Email = request.Email,
                        //VerificationCode = otp,
                    };

                    

                    var resp = await _userProfile.AddAsync(userProfile);

                    if (resp != null)
                    {
                        var templatePath = "EmailTemplate/ConfirmEmail.cshtml";

                        await _emailService.SendFluentEmailTemplate(new EmailRequest()
                        {
                            To = user.Email,
                            Body = $"",
                            Subject = "Confirm Registration",
                            FirstName = $"{request.FirstName}",
                            LastName = $"{request.LastName}",
                            Url = url,
                            //Otp = otp,
                            //Url = $"{_applicationUrlSettings.ConfirmEmailUrl}/{request.Email}?code={otp}",
                        }, templatePath);

                        //return new Response<string>("Check your mail for your OTP to verify your email",
                        //    message: $"User registered successfully.");

                        return new Response<string>(user.Id, "OK");
                       

                        //return new Response<string>("Check your mail for verification link.",
                        //    message: $"User registered successfully.");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                        throw new ApiException("Something went wrong while profiling user");
                    }
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    throw new ApiException($"{result.Errors}");
                }
            }
            else
            {
                throw new ApiException($"Email {request.Email } is already registered.");
            }
        }

        public async Task<Response<bool>> VerifyUser(int otp)
        {
            var user = await _userProfile.GetUserByOtpAsync(otp);

            if (user == null) return new Response<bool>(false, "unsuccessful");

            user.VerificationCode = 0;

            await _userProfile.UpdateAsync(user);

            var aspUser = await _userManager.FindByEmailAsync(user.Email);
            aspUser.EmailConfirmed = true;

            await _userManager.UpdateAsync(aspUser);

            return new Response<bool>(true, "user verified successfully");
        }
        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            var userProfile = await _userProfile.GetUserByEmailAsync(user.Email);

            if (userProfile == null && user.Email == "superadmin@gmail.com")
            {
                userProfile = await _userProfile.AddAsync(new UserProfile
                {
                    Email = "superadmin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    OtherName = "",
                    PhoneNumber = "08030664564"
                });
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new AuthenticationResponse();

            if (response != null)
            {
                response.Id = user.Id;

                if (userProfile != null)
                {
                    response.UserId = userProfile.Id;
                }

                response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.Email = user.Email;
                response.UserName = user.UserName;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                response.Roles = rolesList.ToList();
                response.IsVerified = user.EmailConfirmed;
                response.TokenExpires = jwtSecurityToken.ValidTo;

                if (user.RefreshTokens != null && user.RefreshTokens.Any(a => a.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();

                    if (activeRefreshToken != null)
                    {
                        response.RefreshToken = activeRefreshToken.Token;
                        response.RefreshTokenExpiration = activeRefreshToken.Expires;
                    }

                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    response.RefreshToken = refreshToken.Token;
                    response.RefreshTokenExpiration = refreshToken.Expires;

                    if (user.RefreshTokens != null)
                    {
                        user.RefreshTokens.Add(refreshToken);
                    }

                    _context.Update(user);
                    _context.SaveChanges();
                }
            }

            return new Response<AuthenticationResponse>(response ?? null!, $"Authenticated {user.UserName}");
        }


        public async Task<Response<ValidateCustomerResponse>> ValidateCustomerAsync(string customerId)
        {
            var user = await _userManager.FindByIdAsync(customerId);

            if (user == null)
            {
                throw new ApiException($"No customer record found with Id:  {customerId}");
            }

            ValidateCustomerResponse response = new ValidateCustomerResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNo,
                DueLoanAmount = ""
            };
            

            return new Response<ValidateCustomerResponse>(response ?? null!);
        }

        public async Task<Response<AuthenticationResponse>> RefreshTokenAsync(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new ApiException($"Token did not match any users.");
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new ApiException($"Token Not Active.");
            }

            var userProfile = await _userProfile.GetUserByEmailAsync(user.Email);

            //Revoke Current Refresh Token
            refreshToken.Revoked = DateTime.UtcNow;

            //Generate new Refresh Token and save to Database
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.UserId = userProfile.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.TokenExpires = jwtSecurityToken.ValidTo;
            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.Expires;

            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<Response<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new Response<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task<Response<bool>> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var userProfile = await _userProfile.GetUserByEmailAsync(user.Email);

            if (user == null) throw new ApiException($"No Accounts Registered with {model.Email}.");

            var role = await _userManager.GetRolesAsync(user);

            var url = await GenerateForgotPasswordUrl(user, role.FirstOrDefault() ?? null!);
            var templatePath = "EmailTemplate/ResetPassword.cshtml";

            await _emailService.SendFluentEmailTemplate(new EmailRequest()
            {
                To = user.Email,
                Body = $"",
                Subject = "Reset Password",
                FirstName = $"{userProfile.FirstName}",
                LastName = $"{userProfile.LastName}",
                Url = url
            }, templatePath);

            return new Response<bool>(true, message: $"Mail sent successfully!");
        }

        public async Task<Response<string>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            if (account == null) throw new ApiException($"No Accounts Registered with {model.Email}.");

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(account, token, model.Password);

            if (result.Succeeded)
            {
                return new Response<string>(model.Email, message: $"Password reset successfully.");
            }
            else
            {
                return new Response<string>(message: String.Join(",", result.Errors.Select(x => x.Description)));
            }
        }

        public List<Guid> GetUserIdsByRole(string role)
        {
            var aspUsersEmail = _userManager.GetUsersInRoleAsync(role).Result.Select(x => x.Email).ToList();
            var userIds = _userProfile.GetUserIdsByEmail(aspUsersEmail).ToList();

            return userIds;
        }

        public async Task<Response<string>> ResendVerificationMail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) throw new ApiException($"{email} is not registered yet.");

            if (user.EmailConfirmed == true) throw new ApiException($"{email} has already been confirmed.");

            var userProfile = await _userProfile.GetUserByEmailAsync(user.Email);

            if (userProfile != null)
            {
                //var otp = GenerateOTP();
                var url = await GenerateVerificationEmail(user);

                //userProfile.VerificationCode = otp;
                //await _userProfile.UpdateAsync(userProfile);

                var templatePath = "EmailTemplate/ConfirmEmail.cshtml";

                await _emailService.SendFluentEmailTemplate(new EmailRequest()
                {
                    To = userProfile.Email,
                    Body = $"",
                    Subject = "Confirm Registration",
                    FirstName = $"{userProfile.FirstName}",
                    LastName = $"{userProfile.LastName}",
                    Url = url,
                    //Otp = otp,
                    //Url = $"{_applicationUrlSettings.ConfirmEmailUrl}{userProfile.Email}?code={otp}",
                }, templatePath);

                return new Response<string>($"A verification mail has been re-sent to {userProfile.Email}. Click on the link to confirm your email.",
                    message: "successful");
            }
            else
            {
                throw new ApiException($"Profile not found for {email}");
            }
        }

        public async Task<string> GetUserRoleByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            string rolename = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? null!;

            return rolename;
        }

        public async Task<string> GetUserRoleById(int userId)
        {
            var userprofile = await _userProfile.GetByIdAsync(userId);
            var user = await _userManager.FindByEmailAsync(userprofile.Email);
            string rolename = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? null!;

            return rolename;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                //CreatedByIp = ipAddress
            };
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userProfile = await _userProfile.GetUserByEmailAsync(user.Email);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", userProfile.Id.ToString()),
                new Claim("rol", roles.FirstOrDefault() ?? null!),
                //new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private async Task<string> GenerateVerificationEmail(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var _enpointUri = new Uri(_applicationUrlSettings.ConfirmEmailUrl);
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        private int GenerateOTP()
        {
            int min = 1000;
            int max = 9999;
            var random = new Random();
            var otp = random.Next(min, max);

            return otp;
        }

        private async Task<string> GenerateForgotPasswordUrl(ApplicationUser user, string role)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var _enpointUri = new Uri(_applicationUrlSettings.ForgetPasswordUrl);
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "email", user.Email);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "role", role);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }



        public async Task<Response<string>> CreateGeminiAccountAsync(CreateAccountRequest request)
        {
            
            CreateGeminiAccountRequest geminiAccountRequest = new CreateGeminiAccountRequest();
            Application.DTOs.Gemini.CustData customerData = new Application.DTOs.Gemini.CustData();
            EmployerDto employer = new EmployerDto();
            InfoDto info = new InfoDto();
            KinDto kin = new KinDto();
            IdentificationDto identification = new IdentificationDto();


            if (request.CustData != null)
            {
                //prepare payload for gemini api
                //customer data

                customerData.Address = request.CustData.Address;
                customerData.City = request.CustData.City;
                customerData.CustGroupCode = request.CustData.CustGroupCode;
                customerData.LGACode = request.CustData.LGACode;
                customerData.CountryCode = request.CustData.CountryCode;
                customerData.StateCode = request.CustData.StateCode;
                customerData.BirthDate = request.CustData.BirthDate;
                customerData.CustomerID = request.CustData.CustomerID;
                customerData.Email = request.CustData.Email;
                customerData.FirstName = request.CustData.FirstName;
                customerData.LastName = request.CustData.LastName;
                customerData.Landmark = request.CustData.Landmark;
                customerData.Gender = request.CustData.Gender;
                customerData.MaidenName = request.CustData.MaidenName;
                customerData.MaritalStatus = request.CustData.MaritalStatus;
                customerData.PhoneNo = request.CustData.PhoneNo;

                geminiAccountRequest.CustData = customerData;

                //employer
                employer.TaxNo = request.CustData.Employer.TaxNo;
                employer.EducationLevel = request.CustData.Employer.TaxNo;
                employer.EmployedDate = request.CustData.Employer.EmployedDate;
                employer.PayDay = request.CustData.Employer.EmployedDate;
                employer.EmploymentStatus = request.CustData.Employer.EmploymentStatus;
                employer.EmployerName = request.CustData.Employer.EmployerName;
                employer.StateCode = request.CustData.Employer.StateCode;
                employer.SectorCode = request.CustData.Employer.SectorCode;
                employer.Address = request.CustData.Employer.Address;
                employer.City = request.CustData.Employer.City;
                employer.Email = request.CustData.Employer.Email;
                employer.Landmark = request.CustData.Employer.Landmark;
                employer.LGACode = request.CustData.Employer.LGACode;
                employer.MonthlyIncome = request.CustData.Employer.MonthlyIncome;
                employer.StaffID = request.CustData.Employer.StaffID;
                employer.PensionNo = request.CustData.Employer.PensionNo;
                employer.TelephoneNo = request.CustData.Employer.TelephoneNo;

                customerData.Employer = employer;

                //kin
                kin.PhoneNo = request.CustData.Kin.PhoneNo;
                kin.Email = request.CustData.Kin.Email;
                kin.City = request.CustData.Kin.City;
                kin.StateCode = request.CustData.Kin.StateCode;
                kin.Address = request.CustData.Kin.Address;
                kin.KinName = request.CustData.Kin.KinName;
                kin.Landmark = request.CustData.Kin.Landmark;
                kin.LGACode = request.CustData.Kin.LGACode;
                kin.Relationship = request.CustData.Kin.Relationship;
                kin.EmployerName = request.CustData.Kin.EmployerName;
                kin.TitleKey = request.CustData.Kin.TitleKey;

                customerData.Kin = kin;

                //info
                info.BankCode = request.CustData.Info.BankCode;
                info.ChildrenCount = request.CustData.Info.ChildrenCount;
                info.LoanRepayment = request.CustData.Info.LoanRepayment;
                info.BVNo = request.CustData.Info.BVNo;
                info.AccountNo = request.CustData.Info.AccountNo;
                info.BankCode = request.CustData.Info.BankCode;
                info.ChildrenCount = request.CustData.Info.ChildrenCount;
                info.HouseholdCount = request.CustData.Info.HouseholdCount;
                info.ResidentialStatus = request.CustData.Info.ResidentialStatus;

                customerData.Info = info;

                //identification
                identification.ExpiryDate = request.CustData.Identification.ExpiryDate;
                identification.IssuedDate = request.CustData.Identification.IssuedDate;
                identification.IdentificationNo = request.CustData.Identification.IdentificationNo;
                identification.OtherIdentification = request.CustData.Identification.OtherIdentification;
                identification.Identification = request.CustData.Identification.Identification;

                customerData.Identification = identification;


                //call gemini create account api 
                string token = _geminiApiSettings.ApiToken;
                var uri = _geminiApiSettings.OpenAccountUrl;
                var payload = JsonConvert.SerializeObject(geminiAccountRequest);
                var apiReponse = ApiClient.PostWithTokenAsync(uri, payload, token);
                var geminiResponse = JsonConvert.DeserializeObject<CreateGeminiAccountResponse>(apiReponse.Result);

                if (geminiResponse.ResponseCode == "00")
                {

                    //save user details to local user table
                    RegisterRequest registerRequest = new RegisterRequest
                    {
                        FirstName = request.CustData.FirstName,
                        LastName = request.CustData.LastName,
                        OtherName = request.CustData.MiddleName,
                        BirthDate = request.CustData.BirthDate,
                        Email = request.CustData.Email,
                        PhoneNo = request.CustData.PhoneNo,
                        Password = request.CustData.Password,
                        ConfirmPassword = request.CustData.ConfirmPassword,
                        Address = request.CustData.Address,
                        City = request.CustData.City,
                        CountryCode = request.CustData.CountryCode,
                        CustGroupCode = request.CustData.CustGroupCode,
                        Gender = request.CustData.Gender,
                        CustomerID = request.CustData.CustomerID,
                        LGACode = request.CustData.LGACode,
                        Landmark = request.CustData.Landmark,
                        MaidenName = request.CustData.MaidenName,
                        MaritalStatus = request.CustData.MaritalStatus,
                        StateCode = request.CustData.StateCode,
                        Title = request.CustData.Title,
                        RoleId = request.CustData.RoleId

                    };

                    //save to user table and get user id
                    var response = await RegisterAsync(registerRequest);
                    var userId = response.Data;

                    if(!string.IsNullOrEmpty(userId)) 
                    {
                        //save employee
                        CreateEmployerCommand employerCommand = new CreateEmployerCommand();
                        employerCommand.TaxNo = request.CustData.Employer.TaxNo;
                        employerCommand.EducationLevel = request.CustData.Employer.TaxNo;
                        employerCommand.EmployedDate = request.CustData.Employer.EmployedDate;
                        employerCommand.PayDay = request.CustData.Employer.EmployedDate;
                        employerCommand.EmploymentStatus = request.CustData.Employer.EmploymentStatus;
                        employerCommand.EmployerName = request.CustData.Employer.EmployerName;
                        employerCommand.StateCode = request.CustData.Employer.StateCode;
                        employerCommand.SectorCode = request.CustData.Employer.SectorCode;
                        employerCommand.Address = request.CustData.Employer.Address;
                        employerCommand.City = request.CustData.Employer.City;
                        employerCommand.Email = request.CustData.Employer.Email;
                        employerCommand.Landmark = request.CustData.Employer.Landmark;
                        employerCommand.LGACode = request.CustData.Employer.LGACode;
                        employerCommand.MonthlyIncome = request.CustData.Employer.MonthlyIncome;
                        employerCommand.StaffID = request.CustData.Employer.StaffID;
                        employerCommand.PensionNo = request.CustData.Employer.PensionNo;
                        employerCommand.TelephoneNo = request.CustData.Employer.TelephoneNo;
                        employerCommand.UserId = Guid.Parse(userId);
                        //Response<int> resp = await _mediator.Send(employerCommand);
                         await _mediator.Send(employerCommand);



                       //save kin 
                       CreateKinCommand kinCommand = new CreateKinCommand();
                       kinCommand.PhoneNo = request.CustData.Kin.PhoneNo;
                       kinCommand.Email = request.CustData.Kin.Email;
                       kinCommand.City = request.CustData.Kin.City;
                       kinCommand.StateCode = request.CustData.Kin.StateCode;
                       kinCommand.Address = request.CustData.Kin.Address;
                       kinCommand.KinName = request.CustData.Kin.KinName;
                       kinCommand.Landmark = request.CustData.Kin.Landmark;
                       kinCommand.LGACode = request.CustData.Kin.LGACode;
                       kinCommand.Relationship = request.CustData.Kin.Relationship;
                       kinCommand.EmployerName = request.CustData.Kin.EmployerName;
                       kinCommand.TitleKey = request.CustData.Kin.TitleKey;
                       kinCommand.UserId = Guid.Parse(userId);
                       await _mediator.Send(kinCommand);
                        


                        //save info
                        CreateInfoCommand infoCommand = new CreateInfoCommand();
                        infoCommand.ChildrenCount = request.CustData.Info.ChildrenCount;
                        infoCommand.BankCode = request.CustData.Info.BankCode;
                        infoCommand.LoanRepayment = request.CustData.Info.LoanRepayment;
                        infoCommand.BVNo = request.CustData.Info.BVNo;
                        infoCommand.AccountNo = request.CustData.Info.AccountNo;
                        infoCommand.BankCode = request.CustData.Info.BankCode;
                        infoCommand.ChildrenCount = request.CustData.Info.ChildrenCount;
                        infoCommand.HouseholdCount = request.CustData.Info.HouseholdCount;
                        infoCommand.ResidentialStatus = request.CustData.Info.ResidentialStatus;
                        infoCommand.UserId = Guid.Parse(userId);
                        await _mediator.Send(infoCommand);
                       

                        //identification
                        CreateIdentificationCommand identificationCommand = new CreateIdentificationCommand();
                        identificationCommand.ExpiryDate = request.CustData.Identification.ExpiryDate;
                        identificationCommand.IssuedDate = request.CustData.Identification.IssuedDate;
                        identificationCommand.IdentificationNo = request.CustData.Identification.IdentificationNo;
                        identificationCommand.OtherIdentification = request.CustData.Identification.OtherIdentification;
                        identificationCommand.Identification = request.CustData.Identification.Identification;
                        identificationCommand.UserId = Guid.Parse(userId);
                        await _mediator.Send(identificationCommand);

                        return new Response<string>(userId, message: "Account was created successfully");

                    }

                    return new Response<string>("", message: "Account was not created successfully");

                }
                return new Response<string>("", message: "Account was not created successfully");


            }
            return new Response<string>("", message: "Please enter required fields");

        }

    }
}
