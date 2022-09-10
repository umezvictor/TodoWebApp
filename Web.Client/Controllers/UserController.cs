using Microsoft.AspNetCore.Mvc;
using Web.Client.Services;
using Web.Client.ViewModel;

namespace Web.Client.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public UserController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(SignupViewModel model)
        {
           
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Error = "Please fill the form correctly";
                    return View();
                }

                var response = await _userManagementService.CreateUser(model);
                if (response.IsSuccessful)
                {
                    //send account activation email to user (in real world case)
                   
                    ViewBag.Success = "Your account has been created successfully";

                    return View();

                }
                ViewBag.Error = response.Message;
                return View();
            }
            catch
            {

                ViewBag.Error = "Error occured creating user account";
                return View();
            }
           
        }
    }
}
