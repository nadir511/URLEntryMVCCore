using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.CustomerVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> UpdateCustomer(CustomerVM CustomerInfo);
        Task<bool> IsCustomerExist(string CustomerName);
        Task<bool> IsCustomerExistOnEdit(string CustomerName, int Id);
        bool SaveCustomer(CustomerVM CustomerInfo);
        Task<List<CustomerTbl>> ListOfCustomers();
        Task<List<PointCategory>> ListOfPointCategories();
        Task<CustomerVM> GetCustomerById(int id);
        Task<bool> DeleteCustomer(int Id);
        Task<List<UsersVM>> GetUsersByCustomerId(int CustomerId);
        Task<List<UrlVM>> ListOfPointsAgainstCustomer(int customerId);
        Task<List<UsersVM>> ListOfUsersAgainstCustomer(int customerId);
        Task<List<(string value, string text)>> ListOfSocialProfByCustomer(int customerId);
    }
}
