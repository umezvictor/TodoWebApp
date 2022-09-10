using Web.Client.Responses;
using Web.Client.ViewModel;

namespace Web.Client.Services
{
    public interface IUserManagementService
    {
        Task<ServiceResponse> CreateUser(SignupViewModel userVM);
    }
}