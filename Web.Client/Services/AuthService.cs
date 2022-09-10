using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Web.Client.Models;
using Web.Client.Responses;
using Web.Client.ViewModel;

namespace Web.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<AuthResponse> ValidateUser(LoginViewModel loginVM)
        {
            AuthResponse serviceResponse = new AuthResponse();
            try
            {

                //check if user exists
                var user = await _userManager.FindByEmailAsync(loginVM.Email.Trim());
                if (user == null)
                {
                    serviceResponse.Message = "User does not exist";
                    serviceResponse.IsSuccessful = false;
                    return serviceResponse;
                }


                var authResponse = await _signInManager.PasswordSignInAsync(loginVM.Email.Trim(), loginVM.Password.Trim(), false, false);
                if (authResponse.Succeeded)
                {
                    //return user object in payload                  
                    serviceResponse.IsSuccessful = true;
                    serviceResponse.Payload = JsonConvert.SerializeObject(user);
                    return serviceResponse;

                }
                serviceResponse.Message = "Invalid email or password";
                serviceResponse.IsSuccessful = false;
                return serviceResponse;
            }
            catch
            {

                serviceResponse.Message = "An error occured";

                serviceResponse.IsSuccessful = false;
                return serviceResponse;
            }

        }

    }
}
