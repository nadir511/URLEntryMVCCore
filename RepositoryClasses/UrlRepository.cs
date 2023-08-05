using Microsoft.EntityFrameworkCore;
using URLEntryMVC.ApplicationConstants;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.BusinessReviewVM;
using URLEntryMVC.ViewModel.CustomerVM;
using URLEntryMVC.ViewModel.PointCategoryVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.RepositoryClasses
{
    public class UrlRepository : IUrlRepository
    {
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;

        public UrlRepository(DataContext _dataContext, ICustomerRepository customerRepository)
        {
            _db = _dataContext;
            _customerRepository = customerRepository;
        }
        public void UpdateLink(SaveUrlVM PointInfo)
        {
            try
            {
                var savePointInfo = _db.UrlTbls.Where(x => x.Id == PointInfo.Id).FirstOrDefault();

                if (savePointInfo != null)
                {
                    savePointInfo.Id = PointInfo.Id;
                    if (PointInfo.EditType != AppConstant.MultiEdit)
                    {
                        savePointInfo.SaveInLibrary = PointInfo.SaveInLibrary;
                    }
                    savePointInfo.UrlLink = PointInfo.UrlLink;
                    savePointInfo.DomainLink = PointInfo.DomainLink;
                    savePointInfo.CustomerIdFk = PointInfo.CustomerId;
                    savePointInfo.CustomerPointName = PointInfo.CustomerPointName;
                    savePointInfo.ManagementName = PointInfo.ManagementName;
                    savePointInfo.PointCategoryIdFk = PointInfo.PointCategoryId;
                    savePointInfo.Subject = PointInfo.Subject;
                    savePointInfo.Body = PointInfo.Text;
                    savePointInfo.CustomerNotes = PointInfo.CustomerNotes;
                }
                var EmailInfo = _db.PointEmails.Where(x => x.PointIdFk == PointInfo.Id).FirstOrDefault();
                if (EmailInfo != null)
                    EmailInfo.Email = PointInfo.allEmailsStr;
                _db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsLinkExist(string url)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.UrlLink == url.Trim()).FirstOrDefaultAsync();
                return result == null ? false : true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsLinkExistOnEdit(string url, int Id)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.UrlLink == url.Trim() && x.Id != Id).FirstOrDefaultAsync();
                return result == null ? false : true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsPointExistForCustomer(string customerPoint, int customerId)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.CustomerPointName == customerPoint.Trim() && x.CustomerIdFk == customerId).FirstOrDefaultAsync();
                return (result == null ? false : true);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsPointExistForCustomerOnEdit(string customerPoint, int customerId, int PointId)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.CustomerPointName == customerPoint.Trim() && x.CustomerIdFk == customerId && x.Id != PointId).FirstOrDefaultAsync();
                return (result == null ? false : true);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsManagementNameExistForCustomer(string PointManagementName, int CustomerId)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.ManagementName == PointManagementName.Trim() && x.CustomerIdFk == CustomerId).FirstOrDefaultAsync();
                return (result == null ? false : true);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> IsManagementNameExistOnEditForCustomer(string PointManagementName, int CustomerId, int PointId)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.ManagementName == PointManagementName.Trim() && x.CustomerIdFk == CustomerId && x.Id != PointId).FirstOrDefaultAsync();
                return (result == null ? false : true);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SaveLink(SaveUrlVM PointInfo)
        {
            try
            {
                var savePointInfo = new UrlTbl()
                {
                    SaveInLibrary = PointInfo.SaveInLibrary,
                    UrlLink = PointInfo.UrlLink,
                    DomainLink = PointInfo.DomainLink,
                    CustomerIdFk = PointInfo.CustomerId,
                    CustomerPointName = PointInfo.CustomerPointName,
                    ManagementName = PointInfo.ManagementName,
                    PointCategoryIdFk = PointInfo.PointCategoryId,
                    Subject = PointInfo.Subject,
                    Body = PointInfo.Text,
                    CreationDate = DateTime.UtcNow
                };
                _db.UrlTbls.Add(savePointInfo);
                _db.SaveChanges();
                if (!string.IsNullOrWhiteSpace(PointInfo.allEmailsStr))
                {
                    var pointEmail = new PointEmail()
                    {
                        PointIdFk = savePointInfo.Id,
                        Email = PointInfo.allEmailsStr
                    };
                    _db.PointEmails.Add(pointEmail);
                    _db.SaveChanges();
                }
                if (PointInfo.PointCategoryId == AppConstant.BusinessReviewPointId && PointInfo.businessReviewPoints != null)
                {
                    bool isFirstIteration = true; // Flag to track the first iteration
                    foreach (var item in PointInfo.businessReviewPoints)
                    {
                        if (item.PointUrl != null && item.DelayTimeInMinuts != null)
                        {
                            var addBusinessPoint = new BusinessReviewPoint()
                            {
                                PointUrl = item.PointUrl,
                                CustomerIdFk = PointInfo.CustomerId,
                                IsCurrentlyActive = isFirstIteration == true ? true : false,
                                UrlIdFk = savePointInfo.Id,
                                DelayTimeInMinuts = item.DelayTimeInMinuts,
                                DatePointer = isFirstIteration == true ? savePointInfo.CreationDate.Value.AddMinutes(Convert.ToDouble(item.DelayTimeInMinuts)) : null
                            };
                            _db.BusinessReviewPoints.Add(addBusinessPoint);
                            _db.SaveChanges();
                        }
                        isFirstIteration = false; // Set the flag to false for subsequent iterations
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<getListOfPoints>> ListOfLinks()
        {
            try
            {
                return await _db.GetListOfPoints.FromSqlRaw("exec getListOfPoints").ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<BusinessReviewPoints>> GetListOfDummyBrPoints(int pointId)
        {
            if (pointId<=0)
            {
                List<BusinessReviewPoints> businessReviewUrls = new List<BusinessReviewPoints>();
                for (int i = 1; i < 7; i++)
                {
                    BusinessReviewPoints businessReviewUrl = new BusinessReviewPoints();
                    businessReviewUrl.UrlName = "URL-" + i;
                    businessReviewUrls.Add(businessReviewUrl);
                }
                return businessReviewUrls;
            }
            else
            {
                List<BusinessReviewPoints> businessReviewUrls = await _db.BusinessReviewPoints.Where(x => x.UrlIdFk == pointId).Select(x => new BusinessReviewPoints
                {
                    BusinessPointId = x.BusinessPointId,
                    PointUrl = x.PointUrl,
                    DelayTimeInMinuts = x.DelayTimeInMinuts,
                    DelayTimeInHours = x.DelayTimeInHours,
                    DatePointer = x.DatePointer,
                    IsCurrentlyActive = x.IsCurrentlyActive
                }).ToListAsync();
                return businessReviewUrls;
            }
        }
        public async Task<UrlTbl?> GetUrlById(int Id)
        {
            try
            {
                return await _db.UrlTbls.Where(x => x.Id == Id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string?> GetEmailsByPointId(int PointId)
        {
            try
            {
                return await _db.PointEmails.Where(x => x.PointIdFk == PointId).Select(x => x.Email).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteUrl(int Id)
        {
            try
            {
                var UrlInfo = _db.UrlTbls.Where(x => x.Id == Id).FirstOrDefault();
                if (UrlInfo != null)
                {
                    var pointEmail = _db.PointEmails.Where(x => x.PointIdFk == UrlInfo.Id).FirstOrDefault();
                    if (pointEmail != null)
                        _db.PointEmails.Remove(pointEmail);
                    _db.UrlTbls.Remove(UrlInfo);
                    var BrPoints = _db.BusinessReviewPoints.Where(x => x.UrlIdFk == Id).ToList();
                    BrPoints.ForEach(x =>
                    {
                        x.UrlIdFk = null;
                    });
                }
                _db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<SaveUrlVM> getPointInfoOnEdit(int PointId)
        {
            try
            {
                var obj = await GetUrlById(PointId);
                var customer = await _customerRepository.ListOfCustomers();
                var pointCategory = await _customerRepository.ListOfPointCategories();
                SaveUrlVM editUrlVM = new SaveUrlVM();
                editUrlVM.CustomerList = customer.Select(x => new CustomerInfo
                {
                    CustomerId = x.Id,
                    CustomerName = x.CustomerName
                }).ToList();
                editUrlVM.PointCategoryList = pointCategory.Select(x => new PointCategoryInfo
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();
                editUrlVM.Id = obj.Id;
                editUrlVM.SaveInLibrary = obj.SaveInLibrary ?? false;
                editUrlVM.UrlLink = obj.UrlLink;
                editUrlVM.DomainLink = obj.DomainLink;
                editUrlVM.CustomerPointName = obj.CustomerPointName;
                editUrlVM.ManagementName = obj.ManagementName;
                editUrlVM.CustomerId = obj.CustomerIdFk ?? 0;
                editUrlVM.PointCategoryId = obj.PointCategoryIdFk;
                editUrlVM.Subject = obj.Subject;
                editUrlVM.Text = obj.Body;
                string[] emails = new string[3];

                if (obj.PointCategoryIdFk == AppConstant.EmailContractPointId)
                {
                    var emailsStr = await GetEmailsByPointId(PointId);
                    if (emailsStr != null)
                        emails = emailsStr.Split(',');
                    editUrlVM.Email1 = emails[0];
                    editUrlVM.Email2 = emails.ElementAtOrDefault(1) != null ? emails[1] : null;
                    editUrlVM.Email3 = emails.ElementAtOrDefault(2) != null ? emails[2] : null;
                }

                return editUrlVM;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
