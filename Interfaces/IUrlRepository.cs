using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.BusinessReviewVM;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Interfaces
{
    public interface IUrlRepository
    {
        Task<bool> UpdateLink(SaveUrlVM UrlInfo, bool isCustomerRoleUpdate);
        Task<bool> IsLinkExist(string url);
        Task<bool> IsPointExistForCustomer(string CustomerPointName,int CustomerId);
        Task<bool> IsManagementNameExistForCustomer(string PointManagementName, int CustomerId);
        Task<bool> IsManagementNameExistOnEditForCustomer(string PointManagementName, int CustomerId, int PointId);
        Task<bool> IsLinkExistOnEdit(string url,int Id);
        Task<bool> IsPointExistForCustomerOnEdit(string CustomerPointName, int CustomerId,int PointId);
        void SaveLink(SaveUrlVM UrlInfo);
        Task<List<getListOfPoints>> ListOfLinks();
        Task<UrlTbl?> GetUrlById(int id);
        Task<string?> GetEmailsByPointId(int PointId);
        Task<List<BusinessReviewPoints>> GetListOfDummyBrPoints(int pointId);
        Task<SaveUrlVM> getPointInfoOnEdit(int PointId);
        void DeleteUrl(int Id);
    }
}
