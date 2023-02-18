using URLEntryMVC.Entities;

namespace URLEntryMVC.Interfaces
{
    public interface IUrlRepository
    {
        void UpdateLink(UrlTbl UrlInfo);
        Task<bool> IsLinkExist(string url);
        Task<bool> IsUrlExistForCustomer(string url,int Id);
        Task<bool> IsLinkExistOnEdit(string url,int Id);
        void SaveLink(UrlTbl UrlInfo);
        Task<List<UrlTbl>> ListOfLinks();
        Task<UrlTbl> GetUrlById(int id);
        void DeleteUrl(int Id);
    }
}
