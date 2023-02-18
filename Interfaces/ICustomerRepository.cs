using URLEntryMVC.Entities;

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
    }
}
