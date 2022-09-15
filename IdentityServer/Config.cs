using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
          new Client[]
          {
                   new Client
                   {
                        ClientId = "a93d9ff5-fc41-401d-9007-6501553fbeaa",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("rDxKG5Rm6bW3APA4yk7P+Q==".Sha256())
                        },
                        AllowedScopes = { "todoAPI" }
                   }
          };

        public static IEnumerable<ApiScope> ApiScopes =>
           new ApiScope[]
           {
               //name: todoAPI    description: Todo API
               new ApiScope("todoAPI", "Todo API")
           };

        public static IEnumerable<ApiResource> ApiResources =>
          new ApiResource[]
          {
               //new ApiResource("todoAPI", "Todo API")
          };

        public static IEnumerable<IdentityResource> IdentityResources =>
          new IdentityResource[]
          {
              new IdentityResources.OpenId(),
              new IdentityResources.Profile(),
              new IdentityResources.Address(),
              new IdentityResources.Email(),
              new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" })
          };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "victor",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Victor"),
                        new Claim(JwtClaimTypes.FamilyName, "Umezuruike")
                    }
                }
            };
    }
}
