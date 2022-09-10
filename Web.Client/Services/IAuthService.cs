using Web.Client.Responses;
using Web.Client.ViewModel;

namespace Web.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> ValidateUser(LoginViewModel loginVM);
    }
}