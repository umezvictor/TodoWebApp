using System.Security.Claims;

namespace Web.Client.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        bool ValidateJwtToken(string token);
    }
}