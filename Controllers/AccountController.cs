﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using URLEntryMVC.ApplicationConstants;
using URLEntryMVC.Data;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.EmailServiceVM;
using URLEntryMVC.ViewModel.UrlVM;


namespace URLEntryMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUserExtension> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUserExtension> _signInManager;
        private readonly DataContext _db;
        private readonly IEmailService _mailService;
        private readonly ICustomerRepository _customerRepository;

        public AccountController(UserManager<ApplicationUserExtension> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 SignInManager<ApplicationUserExtension> signInManager,
                                 DataContext dataContext,
                                 IEmailService mailService,
                                 ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = dataContext;
            _mailService = mailService;
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
                    CustomerIdFk = model.CustomerId,
                    EmailConfirmed=false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                    var message = new MessageVM(user.UserName, model.Password, new string[] { user.Email }, "TapThat account Email Confirmation", confirmationLink,null);
                    _mailService.SendEmail(message);
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
        public async Task<ActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
        [HttpGet]
        public IActionResult Error()
        {
            return View();
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
            //registerVMObj.IsPasswordUpdateCall = isPasswordUpdate??false;
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
                UserInfo.CustomerIdFk = registerVMObj.CustomerId;
                //if (registerVMObj.IsPasswordUpdateCall==true)
                //{
                //    UserInfo.PasswordHash = _userManager.PasswordHasher.HashPassword(UserInfo, registerVMObj.Password);
                //}
                var result = await _userManager.UpdateAsync(UserInfo);

                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(UserInfo, registerVMObj.RoleName);
                    await _userManager.AddToRoleAsync(UserInfo, registerVMObj.RoleName);
                    jsonReturnModelObj.isSuccessfull = 1;
                    string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                    //if (registerVMObj.UserId== GetCurrentUserId())
                    //{
                    //    await _signInManager.SignInAsync(UserInfo, isPersistent: false);
                    //}
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

                var userInfo = await _userManager.FindByNameAsync(user.UserName);
                if (result.Succeeded)
                {
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
                    return RedirectToAction("ListOfLinks", "URL",new { pointCategory=AppConstant.AllContractPoint });
                }
                if (userInfo!=null && !userInfo.EmailConfirmed)
                {
                    ModelState.AddModelError("Password", "Please activate your account");
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
        [HttpGet]
        public ActionResult ResetPassword()
        {
            ResetPasswordVM modelObj = new ResetPasswordVM();
            modelObj.Id =GetCurrentUserId();
            return PartialView("_ResetPassword", modelObj); 
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            try
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
                    var user = await _userManager.FindByIdAsync(GetCurrentUserId());
                    if (user != null)
                    {
                        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetPass = await _userManager.ResetPasswordAsync(user, code, resetPasswordVM.Password);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        jsonReturnModelObj.isSuccessfull = 1;
                        string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                        return Json(jsonObj);
                    }
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        [HttpGet]
        public ActionResult ForgetPassword()
        {
            ForgetPasswordVM modelObj = new ForgetPasswordVM();
            return PartialView("_ForgetPassword", modelObj);
        }
        [HttpPost]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordVM modelObj)
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
            var user = await _userManager.FindByEmailAsync(modelObj.Email.Trim());
            if (user == null)
            {
                jsonReturnModelObj.isSuccessfull = 0;
                jsonReturnModelObj.errors.Add("No user exist against this email.");
                string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                return Json(jsonObj);
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callback = Url.Action(nameof(ResetPasswordUsingEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                var message = new MessageVM(user.UserName, null, new string[] { user.Email }, "Reset password link", callback, "forgetPassword");
                _mailService.SendEmail(message);
                jsonReturnModelObj.isSuccessfull = 1;
                string jsonObj = JsonConvert.SerializeObject(jsonReturnModelObj);
                return Json(jsonObj);
            }
        }
        [HttpGet]
        public ActionResult ResetPasswordUsingEmail(string token, string email)
        {
            ResetPasswordUsingEmailVM modelObj = new ResetPasswordUsingEmailVM();
            modelObj.Email = email;
            modelObj.Token = token;
            return View(modelObj);
        }
        [HttpPost]
        public async Task<ActionResult> ResetPasswordUsingEmail(ResetPasswordUsingEmailVM modelObj)
        {
            if (!ModelState.IsValid)
                return View(modelObj);
            var user = await _userManager.FindByEmailAsync(modelObj.Email);
            if (user == null)
                RedirectToAction(nameof(Logout));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, modelObj.Token, modelObj.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(Logout));
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
        public string GetCurrentUserId()
        {
            var userId=_userManager.GetUserId(User);
            return userId;
        }
    }
}
