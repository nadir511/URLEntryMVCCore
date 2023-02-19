using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using URLEntryMVC.Data;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel;

namespace URLEntryMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUserExtension> _userManager;
        private readonly SignInManager<ApplicationUserExtension> _signInManager;
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;

        public AccountController(UserManager<ApplicationUserExtension> userManager,
                                      SignInManager<ApplicationUserExtension> signInManager,
                                      DataContext dataContext,
                                      ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = dataContext;
            _customerRepository = customerRepository;
        }
        public async Task<ActionResult> UsersList()
        {
            List<UsersVM> UsersList = _userManager.Users.Select(x => new UsersVM
            {
                Id = x.Id,
                UserName = x.UserName,
                Email=x.Email
            }).ToList();
            return View(UsersList);
        }
        //[Authorize]
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel registerVMObj = new RegisterViewModel();
            var customer = _customerRepository.ListOfCustomers();
            registerVMObj.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            return PartialView("~/Views/Account/Register.cshtml", registerVMObj);
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            JsonReturnModel jsonReturnModelObj = new JsonReturnModel();
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }
                jsonReturnModelObj.isSuccessfull = 0;
                jsonReturnModelObj.errors.AddRange(errors);
                string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                return Json(jsonObj);
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUserExtension
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    jsonReturnModelObj.isSuccessfull = 1;
                    string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                    return Json(jsonObj);
                }
                
                if (result.Errors.Count() > 0)
                {
                    var errors = result.Errors.Select(x => x.Description).ToList();
                    jsonReturnModelObj.errors.AddRange(errors);
                    jsonReturnModelObj.isSuccessfull = 0;
                    string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                    return Json(jsonObj);
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListOfLinks", "URL");
                }

                ModelState.AddModelError("Password", "Username or password is incorrect!");
            }
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Dispose(true);


            return RedirectToAction("Login");
        }
    }
    public class JsonReturnModel
    {
        public List<string> errors = new List<string>();
        public int isSuccessfull { get; set; }
    }
}
