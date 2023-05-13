using Microsoft.EntityFrameworkCore;
using URLEntryMVC.ApplicationConstants;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.CustomerVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.RepositoryClasses
{
    public class UrlRepository : IUrlRepository
    {
        private readonly DataContext _db;

        public UrlRepository(DataContext _dataContext)
        {
            _db = _dataContext;
        }
        public void UpdateLink(SaveUrlVM PointInfo)
        {
            try
            {
                var savePointInfo= _db.UrlTbls.Where(x => x.Id == PointInfo.Id).FirstOrDefault();

                if (savePointInfo != null)
                {
                    savePointInfo.Id = PointInfo.Id;
                    if (PointInfo.EditType!=AppConstant.MultiEdit)
                    {
                        savePointInfo.SaveInLibrary = PointInfo.SaveInLibrary;
                    }
                    savePointInfo.UrlLink = PointInfo.UrlLink;
                    savePointInfo.DomainLink = PointInfo.DomainLink;
                    savePointInfo.CustomerIdFk = PointInfo.CustomerId;
                    savePointInfo.CustomerPointName = PointInfo.CustomerPointName;
                    savePointInfo.ManagementName= PointInfo.ManagementName;
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
                var result = await _db.UrlTbls.Where(x => x.CustomerPointName == customerPoint.Trim() && x.CustomerIdFk != customerId && x.Id != PointId).FirstOrDefaultAsync();
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
                    SaveInLibrary= PointInfo.SaveInLibrary,
                    UrlLink = PointInfo.UrlLink,
                    DomainLink = PointInfo.DomainLink,
                    CustomerIdFk = PointInfo.CustomerId,
                    CustomerPointName = PointInfo.CustomerPointName,
                    ManagementName= PointInfo.ManagementName,
                    PointCategoryIdFk = PointInfo.PointCategoryId,
                    Subject = PointInfo.Subject,
                    Body = PointInfo.Text
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
                }
                _db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
