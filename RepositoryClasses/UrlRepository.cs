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
             _db.Entry(UrlInfo).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public async Task<bool> IsLinkExist(string url)
        {
            var result = await _db.UrlTbl.Where(x => x.UrlLink == url.Trim()).FirstOrDefaultAsync();
            return result ==null ? false : true;
        }
        public async Task<bool> IsLinkExistOnEdit(string url, int Id)
        {
            var result = await _db.UrlTbl.Where(x => x.UrlLink == url.Trim() && x.Id!= Id).FirstOrDefaultAsync();
            return result == null ? false : true;
        }
        public void SaveLink(UrlTbl UrlInfo)
        {
            _db.Entry(UrlInfo).State = EntityState.Added;
            _db.SaveChanges();
        }
        public async Task<List<UrlTbl>> ListOfLinks()
        {
            return await _db.UrlTbl.ToListAsync();
        }
        public async Task<UrlTbl> GetUrlById(int Id)
        {
            return await _db.UrlTbl.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }
        public void DeleteUrl(int Id)
        {
            var UrlInfo =_db.UrlTbl.Where(x => x.Id == Id).FirstOrDefault();
            if (UrlInfo != null)
                _db.UrlTbl.Remove(UrlInfo);
            _db.SaveChanges();
        }
    }
}
