using Web.Client.DTOs;
using Web.Client.Utils;

namespace Web.Client.Services
{
    public class TodoService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public TodoService(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

             
        public List<Todo> GetTodos()
        {
            var todoUrl = _configuration.GetSection("TodoUrl").Value;
            var tokenResponse = _tokenService.GetApiToken();
            if(tokenResponse != null)
            {
                var token = tokenResponse.access_token;
                var todoResponse = ApiClient.GetAsync(todoUrl, token);
               // var todoGreenPay = 
            }
            

            return new List<Todo>();
        }
    }
}
