using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using URLEntryMVC.ApplicationConstants;
using URLEntryMVC.Data;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.UrlVM;


namespace URLEntryMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUserExtension> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUserExtension> _signInManager;
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;

        public AccountController(UserManager<ApplicationUserExtension> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 SignInManager<ApplicationUserExtension> signInManager,
                                 DataContext dataContext,
                                 ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                Email = x.Email
            }).ToList();
            return View(UsersList);
        }
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult> Register()
        {
            RegisterViewModel registerVMObj = new RegisterViewModel();
            var customer = await _customerRepository.ListOfCustomers();
            registerVMObj.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            registerVMObj.RolesList = _roleManager.Roles.Select(x => new CreateRoleViewModel
            {
                RoleName=x.Name
            }).ToList();
            return PartialView("~/Views/Account/Register.cshtml", registerVMObj);
        }
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
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
                    CustomerIdFk = model.CustomerId
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRoleAsync(user, model.RoleName);
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
        public async Task<ActionResult> EditRegister(string userId)
        {
            EditRegisterViewModel? registerVMObj = new EditRegisterViewModel();
            registerVMObj = _userManager.Users.Where(x => x.Id == userId).Select(x => new EditRegisterViewModel
            {
                UserId = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                CustomerId = x.CustomerIdFk
            }).FirstOrDefault();
            var customer = await _customerRepository.ListOfCustomers();
            registerVMObj.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            registerVMObj.RoleName = role.FirstOrDefault();
            registerVMObj.RolesList = _roleManager.Roles.Select(x => new CreateRoleViewModel
            {
                RoleName = x.Name
            }).ToList();
            return PartialView("~/Views/Account/EditRegister.cshtml", registerVMObj);
        }
        [HttpPost]
        public async Task<ActionResult> EditRegister(EditRegisterViewModel registerVMObj)
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
                var UserInfo =await _userManager.FindByIdAsync(registerVMObj.UserId);
                UserInfo.UserName = registerVMObj.UserName;
                UserInfo.Email= registerVMObj.Email;
                //UserInfo.PasswordHash = _userManager.PasswordHasher.HashPassword(UserInfo,registerVMObj.Password);
                var result = await _userManager.UpdateAsync(UserInfo);

                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(UserInfo, registerVMObj.RoleName);
                    await _userManager.AddToRoleAsync(UserInfo, registerVMObj.RoleName);
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
            return View();
        }
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return Json(1);
                else
                    return Json(0);
            }
            return View("UsersList");
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
            HttpContext.Session.Remove(AppConstant.CustomerLogoStr);
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    var userInfo = await _userManager.FindByNameAsync(user.UserName);
                    var role = await _userManager.GetRolesAsync(userInfo);
                    if (userInfo.CustomerIdFk!=null)
                    {
                        var customerInfo = await _customerRepository.GetCustomerById(userInfo.CustomerIdFk??0);
                        if (customerInfo!=null && customerInfo.CustomerPic!=null)
                        {
                            var base64Image = Convert.ToBase64String(customerInfo.CustomerPic);
                            var Logo = String.Format("data:image/png;base64,{0}", base64Image);
                            HttpContext.Session.SetString(AppConstant.CustomerLogoStr, Logo);
                        }
                    }
                    HttpContext.Session.SetString(AppConstant.loggedInUserRole, role.FirstOrDefault() ?? string.Empty);
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

        public class JsonReturnModel
        {
            public List<string> errors = new List<string>();
            public int isSuccessfull { get; set; }
        }
        #region|Roles Logic|
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList", "Account");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        #endregion
    }
}
