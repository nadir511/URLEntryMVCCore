using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;

namespace URLEntryMVC.RepositoryClasses
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _db;

        public CustomerRepository(DataContext dataContext)
        {
            _db = dataContext;
        }
        public async Task<bool> DeleteCustomer(int Id)
        {
            try
            {
                var custObj=await _db.CustomerTbls.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (custObj != null)
                    _db.CustomerTbls.Remove(custObj);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerTbl> GetCustomerById(int Id)
        {
            try
            {
                return await _db.CustomerTbls.Where(x => x.Id == Id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsCustomerExist(string CustomerName)
        {
            try
            {
                var result = await _db.CustomerTbls.Where(x => x.CustomerName == CustomerName.Trim()).FirstOrDefaultAsync();
                return result == null ? false : true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsLinkCustomerOnEdit(string CustomerName, int Id)
        {
            try
            {
                var result = await _db.CustomerTbls.Where(x => x.CustomerName == CustomerName.Trim() && x.Id!=Id).FirstOrDefaultAsync();
                return result == null ? false : true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CustomerTbl>> ListOfCustomers()
        {
            try
            {
                return await _db.CustomerTbls.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveCustomer(CustomerTbl CustomerInfo)
        {
            try
            {
                _db.Entry(CustomerInfo).State = EntityState.Added;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCustomer(CustomerTbl CustomerInfo)
        {
            try
            {
                _db.Entry(CustomerInfo).State = EntityState.Modified;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
