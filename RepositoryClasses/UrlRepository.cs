using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;

namespace URLEntryMVC.RepositoryClasses
{
    public class UrlRepository:IUrlRepository
    {
        private readonly DataContext _db;

        public UrlRepository(DataContext _dataContext)
        {
            _db = _dataContext;
        }
        public void UpdateLink(UrlTbl UrlInfo)
        {
            try
            {
                _db.Entry(UrlInfo).State = EntityState.Modified;
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
        public void SaveLink(UrlTbl UrlInfo)
        {
            try
            {
                _db.Entry(UrlInfo).State = EntityState.Added;
                _db.SaveChanges();
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
        public async Task<UrlTbl> GetUrlById(int Id)
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
