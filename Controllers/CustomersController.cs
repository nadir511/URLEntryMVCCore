using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel;

namespace URLEntryMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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
        public async Task<ActionResult> DeleteCustomer(int Id)
        {
            try
            {
                await _customerRepository.DeleteCustomer(Id);
                return Json(1);
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
    }
}
