using System.Security.Claims;
using Web.Client.Responses;

namespace Web.Client.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        bool ValidateJwtToken(string token);
        public ApiTokenResponse? GetApiToken();
    }
}