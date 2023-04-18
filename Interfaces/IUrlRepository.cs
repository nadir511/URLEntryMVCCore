﻿using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Interfaces
{
    public interface IUrlRepository
    {
        void UpdateLink(SaveUrlVM UrlInfo);
        Task<bool> IsLinkExist(string url);
        Task<bool> IsPointExistForCustomer(string CustomerPointName,int CustomerId);
        Task<bool> IsLinkExistOnEdit(string url,int Id);
        Task<bool> IsPointExistForCustomerOnEdit(string CustomerPointName, int CustomerId,int PointId);
        void SaveLink(SaveUrlVM UrlInfo);
        Task<List<getListOfPoints>> ListOfLinks();
        Task<UrlTbl?> GetUrlById(int id);
        Task<string?> GetEmailsByPointId(int PointId);
        void DeleteUrl(int Id);
    }
}
