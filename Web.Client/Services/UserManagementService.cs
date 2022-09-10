using Microsoft.AspNetCore.Identity;
using Web.Client.Models;
using Web.Client.Responses;
using Web.Client.ViewModel;

namespace Web.Client.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserManagementService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ServiceResponse> CreateUser(SignupViewModel userVM)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            try
            {
                //check if user exists
                var existingUser = await _userManager.FindByEmailAsync(userVM.Email);
                if (existingUser != null)
                {
                    serviceResponse.IsSuccessful = false;
                    serviceResponse.Message = "User already exists";
                    return serviceResponse;
                }

                //create user object
                var user = new AppUser
                {
                    Email = userVM.Email.Trim(),
                    UserName = userVM.Email.Trim(),
                    FirstName = userVM.FirstName.Trim(),
                    LastName = userVM.LastName.Trim(),

                };

                var creatingUser = await _userManager.CreateAsync(user, userVM.Password.Trim());
                if (creatingUser.Succeeded)
                {
                    serviceResponse.IsSuccessful = true;
                    serviceResponse.Message = "User created successfully";
                    return serviceResponse;
                }


                serviceResponse.IsSuccessful = false;
                serviceResponse.Message = "User was not created successfully";
                return serviceResponse;

            }
            catch
            {

                serviceResponse.IsSuccessful = false;
                serviceResponse.Message = "An error occured";
                return serviceResponse;
            }
        }

    }
}
