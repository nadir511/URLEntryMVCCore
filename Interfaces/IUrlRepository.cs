using URLEntryMVC.Entities;

namespace URLEntryMVC.Interfaces
{
    public interface IUrlRepository
    {
        void UpdateLink(UrlTbl UrlInfo);
        Task<bool> IsLinkExist(string url);
        Task<bool> IsPointExistForCustomer(string CustomerPointName,int CustomerId);
        Task<bool> IsLinkExistOnEdit(string url,int Id);
        Task<bool> IsPointExistForCustomerOnEdit(string CustomerPointName, int CustomerId,int PointId);
        void SaveLink(UrlTbl UrlInfo);
        Task<List<UrlTbl>> ListOfLinks();
        Task<UrlTbl?> GetUrlById(int id);
        void DeleteUrl(int Id);
    }
}
