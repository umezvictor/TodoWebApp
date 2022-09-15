using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Web.Client.Models;
using Web.Client.Services;
using Web.Client.ViewModel;

namespace Web.Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var validationResponse = await _authService.ValidateUser(loginVM);

                if (validationResponse.IsSuccessful)
                {
                    //get user info
                    var user = JsonConvert.DeserializeObject<AppUser>(validationResponse.Payload);
                    if (user != null)
                    {
                        //if ok, fetches some user details and stores in claims
                        List<Claim> claims = new List<Claim>();
                        var email = new Claim("Email", user.Email);
                        var userId = new Claim("Id", user.Id.ToString());

                        claims.Add(email);
                        claims.Add(userId);
                        //generate token
                        var token = _tokenService.GenerateAccessToken(claims);
                        HttpContext.Session.SetString("token", token);
                        HttpContext.Session.SetString("Id", user.Id.ToString());
                        HttpContext.Session.SetString("FirstName", user.FirstName);

                        return RedirectToAction("Index", "Home");

                    }
                }
                //validation failed
                ViewBag.Error = validationResponse.Message;
                return View();
            }
            catch 
            {                
                ViewBag.Error = "An error occured";
                return View();

            }

        }

        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            ClearCookies();
            return RedirectToAction("Login", "Auth");
        }


        private void ClearCookies()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
        }
    }


}
