using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.RepositoryClasses
{
    public class UrlRepository:IUrlRepository
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
                var savePointInfo = new UrlTbl()
                {
                    Id=PointInfo.Id,
                    UrlLink = PointInfo.UrlLink,
                    DomainLink = PointInfo.DomainLink,
                    CustomerIdFk = PointInfo.CustomerId,
                    CustomerPointName = PointInfo.CustomerPointName,
                    PointCategoryIdFk = PointInfo.PointCategoryId,
                    Subject = PointInfo.Subject,
                    Body = PointInfo.Text
                };
                _db.Entry(savePointInfo).State = EntityState.Modified;
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
        public async Task<bool> IsPointExistForCustomer(string customerPoint,int customerId)
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
        public async Task<bool> IsPointExistForCustomerOnEdit(string customerPoint, int customerId,int PointId)
        {
            try
            {
                var result = await _db.UrlTbls.Where(x => x.CustomerPointName == customerPoint.Trim() && x.CustomerIdFk == customerId &&x.Id!= PointId).FirstOrDefaultAsync();
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
                    UrlLink = PointInfo.UrlLink,
                    DomainLink = PointInfo.DomainLink,
                    CustomerIdFk = PointInfo.CustomerId,
                    CustomerPointName = PointInfo.CustomerPointName,
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
        public async Task<List<UrlTbl>> ListOfLinks()
        {
            try
            {
                return await _db.UrlTbls.ToListAsync();
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
                return await _db.PointEmails.Where(x => x.PointIdFk == PointId).Select(x=>x.Email).FirstOrDefaultAsync();
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
                    _db.UrlTbls.Remove(UrlInfo);
                _db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
