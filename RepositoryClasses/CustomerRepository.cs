using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Extensions;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.AccountVM;
using URLEntryMVC.ViewModel.BusinessReviewVM;
using URLEntryMVC.ViewModel.CustomerVM;
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
                List<UrlVM> listOfPointsAgainstCustomer = await _db.UrlTbls.Where(x => x.CustomerIdFk == customerId).Select(x => new UrlVM
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
                    UserName = x.UserName,
                    Email = x.Email
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
                var custObj = await _db.CustomerTbls.Where(x => x.Id == Id).FirstOrDefaultAsync();
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


        public async Task<CustomerVM> GetCustomerById(int Id)
        {
            try
            {
                CustomerVM customerVM = new CustomerVM();
                var customerInfo = await _db.CustomerTbls.Where(x => x.Id == Id).FirstOrDefaultAsync();
                customerVM.Id = Id;
                customerVM.CustomerName = customerInfo.CustomerName;
                customerVM.ContactNumber = customerInfo.ContactNumber;
                customerVM.CustomerEmail = customerInfo.CustomerEmail;
                customerVM.Address = customerInfo.Address;
                customerVM.CustomerPic = customerInfo.CustomerPic;
                customerVM.Instagram = customerInfo.Instagram;
                customerVM.Facebook = customerInfo.Facebook;
                customerVM.Twitter = customerInfo.Twitter;
                customerVM.LinkedIn = customerInfo.LinkedIn;
                customerVM.TikTok = customerInfo.TikTok;
                customerVM.Youtube = customerInfo.Youtube;
                customerVM.Snapchat = customerInfo.Snapchat;

                customerVM.businessReviewUrls = await _db.BusinessReviewPoints.Where(x => x.CustomerIdFk == Id).Select(x => new BusinessReviewUrl
                {
                    BusinessPointId = x.BusinessPointId,
                    PointUrl = x.PointUrl,
                }).ToListAsync();
                if (customerVM.businessReviewUrls.Count < 1)
                {
                    for (int i = 1; i < 7; i++)
                    {
                        BusinessReviewUrl businessReviewUrl = new BusinessReviewUrl();
                        businessReviewUrl.UrlName = "URL-" + i;
                        customerVM.businessReviewUrls.Add(businessReviewUrl);
                    }
                }
                else
                {
                    for (int i = 0; i < customerVM.businessReviewUrls.Count; i++)
                    {
                        customerVM.businessReviewUrls[i].UrlName = "URL-" + i;
                    }
                }
                return customerVM;
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
                var result = await _db.CustomerTbls.Where(x => x.CustomerName == CustomerName.Trim() && x.Id != CustomerId).FirstOrDefaultAsync();
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
        public async Task<List<PointCategory>> ListOfPointCategories()
        {
            try
            {
                return await _db.PointCategories.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool SaveCustomer(CustomerVM customerVM)
        {
            try
            {
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
                    CustomerEmail = customerVM.CustomerEmail,
                    CustomerPic = fileBytes,
                };
                _db.Entry(customer).State = EntityState.Added;
                _db.SaveChanges();
                if (customerVM.businessReviewUrls != null && customerVM.businessReviewUrls.Count > 0)
                {
                    List<BusinessReviewPoint> PointsObj = new List<BusinessReviewPoint>();
                    foreach (var item in customerVM.businessReviewUrls)
                    {
                        BusinessReviewPoint Obj = new BusinessReviewPoint();
                        Obj.PointUrl = item.PointUrl;
                        Obj.IsCurrentlyActive = false;
                        Obj.CustomerIdFk = customer.Id;
                        PointsObj.Add(Obj);
                    }
                    _db.BusinessReviewPoints.AddRange(PointsObj);
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCustomer(CustomerVM customerVM)
        {
            try
            {
                var fileBytes = new byte[] { };
                var ms = new MemoryStream();
                if (customerVM.CustomerLogo != null)
                {
                    customerVM.CustomerLogo.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                else
                {
                    var picByte = await _db.CustomerTbls.Where(x => x.Id == customerVM.Id).Select(x => x.CustomerPic).FirstOrDefaultAsync();
                    fileBytes = picByte;
                }
                var customer = new CustomerTbl()
                {
                    Id = customerVM.Id,
                    CustomerName = customerVM.CustomerName,
                    ContactNumber = customerVM.ContactNumber,
                    CustomerEmail = customerVM.CustomerEmail,
                    Address = customerVM.Address,
                    CustomerPic = fileBytes,
                    Instagram = customerVM.Instagram,
                    Facebook = customerVM.Facebook,
                    Twitter = customerVM.Twitter,
                    LinkedIn = customerVM.LinkedIn,
                    TikTok = customerVM.TikTok,
                    Youtube = customerVM.Youtube,
                    Snapchat = customerVM.Snapchat,
                };
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                if (customerVM.businessReviewUrls != null && customerVM.businessReviewUrls.Count > 0)
                {
                    foreach (var item in customerVM.businessReviewUrls)
                    {
                        if (item.BusinessPointId != null && item.BusinessPointId > 0)
                        {
                            //It means record is alreday in DB and we have to update that record
                            var urlInfo = await _db.BusinessReviewPoints.Where(x => x.BusinessPointId == item.BusinessPointId && x.CustomerIdFk == customerVM.Id).FirstOrDefaultAsync();
                            if (urlInfo != null)
                            {
                                urlInfo.PointUrl = item.PointUrl;
                                await _db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            BusinessReviewPoint Obj = new BusinessReviewPoint();
                            Obj.PointUrl = item.PointUrl;
                            Obj.IsCurrentlyActive = false;
                            Obj.CustomerIdFk = customer.Id;
                            _db.BusinessReviewPoints.Add(Obj);
                            await _db.SaveChangesAsync();
                        }
                    }
                }

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
        public async Task<List<(string value, string text)>> ListOfSocialProfByCustomer(int customerId)
        {
            try
            {
                var customerProfiles = await _db.CustomerTbls.Where(x => x.Id == customerId).Select(x => new CustomerSocialProfileVM
                {
                    Facebook = x.Facebook,
                    Instagram = x.Instagram,
                    Twitter = x.Twitter,
                    LinkedIn = x.LinkedIn,
                    TikTok = x.TikTok,
                    Youtube = x.Youtube,
                    Snapchat = x.Snapchat
                }).FirstOrDefaultAsync();
                List<(string? text, string? value)> profilesList = new List<(string? text, string? value)>
                    {
                        ("Facebook",customerProfiles.Facebook),
                        ("Instagram",customerProfiles.Instagram),
                        ("Twitter" , customerProfiles.Twitter ),
                        ("LinkedIn" , customerProfiles.LinkedIn ),
                        ("TikTok" , customerProfiles.TikTok ),
                        ("Youtube" , customerProfiles.Youtube ),
                        ("Snapchat" , customerProfiles.Snapchat )
                    };
                profilesList = profilesList.Where(x => x.value != null).ToList();
                return profilesList;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
