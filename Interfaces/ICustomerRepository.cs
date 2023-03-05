using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Interfaces
{
    public interface ICustomerRepository
    {
        bool UpdateCustomer(CustomerTbl CustomerInfo);
        Task<bool> IsCustomerExist(string CustomerName);
        Task<bool> IsCustomerExistOnEdit(string CustomerName, int Id);
        bool SaveCustomer(CustomerTbl CustomerInfo);
        Task<List<CustomerTbl>> ListOfCustomers();
        Task<CustomerTbl> GetCustomerById(int id);
        Task<bool> DeleteCustomer(int Id);
        Task<List<UsersVM>> GetUsersByCustomerId(int CustomerId);
        Task<List<UrlVM>> ListOfPointsAgainstCustomer(int customerId);
        Task<List<UsersVM>> ListOfUsersAgainstCustomer(int customerId);
    }
}
