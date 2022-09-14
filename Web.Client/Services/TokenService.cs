using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web.Client.Responses;

namespace Web.Client.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var rsaKey = _configuration["Token:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(rsaKey));


            var jwtToken = new JwtSecurityToken(issuer: _configuration["Token:Issuer"],
                audience: _configuration["Token:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Token:TokenExpiry"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }


        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var rsaKey = _configuration["Token:SecretKey"];
            var key = Encoding.ASCII.GetBytes(rsaKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                //get user id from token
                var defaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);
                if (userId != defaultGuid)
                    return true;
                return false;
            }
            catch
            {
                
                return false;
            }
        }

         public ApiTokenResponse? GetApiToken()
         {
           
            try
            {
                var client = new HttpClient();
                var res = "";               
                var clientId = _configuration.GetSection("ClientId").Value;
                var clientSecret = _configuration.GetSection("ClientSecret").Value;               
                var grantType = _configuration.GetSection("GrantType").Value;               
                var scope = _configuration.GetSection("Scope").Value;               
                var tokenUrl = _configuration.GetSection("TokenUrl").Value; 
                
                var data = new[]
                {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", $"{scope}"),
                new KeyValuePair<string, string>("grant_type", grantType)
                };

                HttpResponseMessage resp = client.PostAsync(tokenUrl, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();               
                if (resp.IsSuccessStatusCode)
                {
                    res = resp.Content.ReadAsStringAsync().Result;
                    var tokenResponse = JsonConvert.DeserializeObject<ApiTokenResponse>(res);
                    
                    return tokenResponse;
                }
                else
                {
                    
                    return null;
                }

            }
            catch
            {               
                return null;
            }



        }
    }
}
