using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Data;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.BusinessReviewVM;
using URLEntryMVC.ViewModel.CustomerVM;

namespace URLEntryMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUrlRepository _urlRepository;
        private readonly DataContext _db;
        private readonly UserManager<ApplicationUserExtension> _userManager;

        public CustomersController(ICustomerRepository customerRepository,
                                    IUrlRepository urlRepository,
                                    DataContext dataContext,
                                    UserManager<ApplicationUserExtension> userManager)
        {
            _customerRepository = customerRepository;
            _urlRepository = urlRepository;
            _db = dataContext;
            _userManager = userManager;
        }
        public async Task<ActionResult<List<CustomerVM>>> CustomerList()
        {
            try
            {
                var CustomerList = await _customerRepository.ListOfCustomers();
                List<CustomerVM> cList = CustomerList.Select(x => new CustomerVM
                {
                    Id = x.Id,
                    CustomerName = x.CustomerName,
                    Address = x.Address,
                    ContactNumber = x.ContactNumber
                }).ToList();
                return View(cList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ActionResult> customerProfile(int? customerId, bool? disableProfile)
        {
            try
            {
                CustomerVM customerVM = new CustomerVM();
                var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
                var cstmrId = 0;
                if (disableProfile == true)
                {
                    cstmrId = (Int32)customerId;
                }
                else
                {
                    cstmrId = (Int32)userInfo.CustomerIdFk;
                }
                customerVM = await _customerRepository.GetCustomerById(cstmrId);
                if (customerVM != null)
                {
                    customerVM.isProfileDisabled = disableProfile ?? false;
                }
                if (disableProfile == true)
                {
                    return PartialView("~/Views/Customers/customerProfile.cshtml", customerVM);
                }
                else
                {
                    return View(customerVM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<ActionResult> customerProfile(CustomerVM customerVM)
        {
            try
            {
                bool IsCustomerSave = await _customerRepository.UpdateCustomer(customerVM);
                return RedirectToAction("customerProfile");
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpGet]
        public ActionResult SaveCustomer()
        {
            try
            {
                CustomerVM customerVM = new CustomerVM();
                return PartialView("~/Views/PartialViews/_AddCustomerModal.cshtml", customerVM);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost]
        public async Task<ActionResult> SaveCustomer(CustomerVM customerVM)
        {
            try
            {
                bool IsCustomerExist = await _customerRepository.IsCustomerExist(customerVM.CustomerName);
                if (IsCustomerExist)
                    return Json(0);
                bool IsCustomerSave = _customerRepository.SaveCustomer(customerVM);
                return Json(1);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpGet]
        public async Task<ActionResult> EditCustomer(int Id)
        {
            try
            {
                CustomerVM customerVM = new CustomerVM();
                customerVM = await _customerRepository.GetCustomerById(Id);

                return PartialView("~/Views/Customers/_EditCustomer.cshtml", customerVM);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditCustomer(CustomerVM customerVM)
        {
            try
            {

                bool IsCustomerExist = await _customerRepository.IsCustomerExistOnEdit(customerVM.CustomerName, customerVM.Id);
                if (IsCustomerExist)
                    return Json(0);

                bool IsCustomerSave = await _customerRepository.UpdateCustomer(customerVM);
                return Json(1);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeleteCustomer(int Id)
        {
            try
            {
                DeleteCustomerVM deleteCustomerVM = new DeleteCustomerVM();
                deleteCustomerVM.customerPoint = await _customerRepository.ListOfPointsAgainstCustomer(Id);
                deleteCustomerVM.customerUsers = await _customerRepository.ListOfUsersAgainstCustomer(Id);
                if ((deleteCustomerVM.customerPoint.Count() + deleteCustomerVM.customerUsers.Count()) > 0)
                {
                    deleteCustomerVM.isDeleted = false;
                }
                else
                {
                    deleteCustomerVM.isDeleted = true;
                }
                return PartialView("~/Views/Customers/_DeleteCustomer.cshtml", deleteCustomerVM);
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public async Task<ActionResult> ConfirmDeleteCustomer(int Id)
        {
            try
            {
                var Links = await _urlRepository.ListOfLinks();
                await _customerRepository.DeleteCustomer(Id);
                return Json(1);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ActionResult> ShowCustomerUsers(int Id)
        {
            try
            {
                DeleteCustomerVM CustomerVMInfo = new DeleteCustomerVM();
                CustomerVMInfo.customerPoint = await _customerRepository.ListOfPointsAgainstCustomer(Id);
                CustomerVMInfo.customerUsers = await _customerRepository.ListOfUsersAgainstCustomer(Id);
                return PartialView("~/Views/Customers/CustomerUsersList.cshtml", CustomerVMInfo);
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
    }
}
