using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.CustomerVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUrlRepository _urlRepository;
        private readonly DataContext _db;

        public CustomersController(ICustomerRepository customerRepository, IUrlRepository urlRepository, DataContext dataContext)
        {
            _customerRepository = customerRepository;
            _urlRepository = urlRepository;
            _db = dataContext;
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
        public ActionResult SaveCustomer()
        {
            CustomerVM customerVM = new CustomerVM();
            return PartialView("~/Views/PartialViews/_AddCustomerModal.cshtml", customerVM);
        }
        [HttpPost]
        public async Task<ActionResult> SaveCustomer(CustomerVM customerVM)
        {
            try
            {
                bool IsCustomerExist = await _customerRepository.IsCustomerExist(customerVM.CustomerName);
                if (IsCustomerExist)
                    return Json(0);
                var ms = new MemoryStream();
                if (customerVM.CustomerLogo != null)
                    customerVM.CustomerLogo.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                var customer = new CustomerTbl()
                {
                    CustomerName = customerVM.CustomerName,
                    ContactNumber = customerVM.ContactNumber,
                    Address = customerVM.Address,
                    CustomerPic = fileBytes,
                };
                bool IsCustomerSave= _customerRepository.SaveCustomer(customer);
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
                var customerInfo = await _customerRepository.GetCustomerById(Id);
                CustomerVM customerVM = new CustomerVM();
                customerVM.Id = customerInfo.Id;
                customerVM.CustomerName = customerInfo.CustomerName;
                customerVM.ContactNumber = customerInfo.ContactNumber;
                customerVM.Address = customerInfo.Address;
                customerVM.CustomerPic = customerInfo.CustomerPic;
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
                var fileBytes=new byte[] { };
                bool IsCustomerExist = await _customerRepository.IsCustomerExistOnEdit(customerVM.CustomerName, customerVM.Id);
                if (IsCustomerExist)
                    return Json(0);
                var ms = new MemoryStream();
                if (customerVM.CustomerLogo != null)
                {
                    customerVM.CustomerLogo.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                else
                {
                    var picByte = _db.CustomerTbls.Where(x => x.Id == customerVM.Id).Select(x => x.CustomerPic).FirstOrDefault();
                    fileBytes = picByte;
                }
                var customer = new CustomerTbl()
                {
                    Id = customerVM.Id,
                    CustomerName = customerVM.CustomerName,
                    ContactNumber = customerVM.ContactNumber,
                    Address = customerVM.Address,
                    CustomerPic = fileBytes,
                };
                bool IsCustomerSave = _customerRepository.UpdateCustomer(customer);
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
            DeleteCustomerVM deleteCustomerVM = new DeleteCustomerVM();
            deleteCustomerVM.customerPoint = await _customerRepository.ListOfPointsAgainstCustomer(Id);
            deleteCustomerVM.customerUsers = await _customerRepository.ListOfUsersAgainstCustomer(Id);
            if ((deleteCustomerVM.customerPoint.Count() + deleteCustomerVM.customerUsers.Count())>0)
            {
                deleteCustomerVM.isDeleted = false;
            }
            else
            {
                deleteCustomerVM.isDeleted = true;
            }
            return PartialView("~/Views/Customers/_DeleteCustomer.cshtml", deleteCustomerVM);
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
