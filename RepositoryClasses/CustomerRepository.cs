using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.RepositoryClasses
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _db;
        private readonly UserManager<ApplicationUserExtension> _userManager;

        public CustomerRepository(DataContext dataContext, UserManager<ApplicationUserExtension> userManager)
        {
            _db = dataContext;
            _userManager = userManager;
        }
        public async Task<List<UrlVM>> ListOfPointsAgainstCustomer(int customerId)
        {
            try
            {
                List<UrlVM> listOfPointsAgainstCustomer =await _db.UrlTbls.Where(x => x.CustomerIdFk == customerId).Select(x => new UrlVM
                {
                    UrlLink = x.UrlLink,
                    CustomerPointName = x.CustomerPointName
                }).ToListAsync();
                return listOfPointsAgainstCustomer;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<UsersVM>> ListOfUsersAgainstCustomer(int customerId)
        {
            try
            {
                List<UsersVM> listOfUsersAgainstCustomer = await _userManager.Users.Where(x => x.CustomerIdFk == customerId).Select(x => new UsersVM
                {
                    UserName=x.UserName,
                    Email=x.Email
                }).ToListAsync();
                return listOfUsersAgainstCustomer;
            }
            catch (Exception)
            {

                throw;
            }
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

        public async Task<bool> IsCustomerExistOnEdit(string CustomerName, int CustomerId)
        {
            try
            {
                var result = await _db.CustomerTbls.Where(x => x.CustomerName == CustomerName.Trim() && x.Id!= CustomerId).FirstOrDefaultAsync();
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
        public async Task<List<UsersVM>> GetUsersByCustomerId(int CustomerId)
        {
            try
            {
                List<UsersVM> usersListByCustomer = await _userManager.Users.Where(x => x.CustomerIdFk == CustomerId).Select(x => new UsersVM
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email
                }).ToListAsync();
                return usersListByCustomer;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
