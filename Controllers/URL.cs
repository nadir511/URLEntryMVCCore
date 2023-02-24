using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.Data;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

    public class URL : Controller
    {
        private readonly IUrlRepository urlRepositoryObj;
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;
        private string domainLink = "http://tapthat.online/_";

        public URL(IUrlRepository urlRepository, DataContext db,ICustomerRepository customerRepository)
        {
            urlRepositoryObj = urlRepository;
            _db = db;
            _customerRepository = customerRepository;
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UrlVM>>> ListOfLinks()
        {
            var Links = await urlRepositoryObj.ListOfLinks();
            ViewBag.TotalPoints = _db.UrlTbls.ToList().Count();
            ViewBag.TotalCustomers = _db.CustomerTbls.ToList().Count();
            List<UrlVM> UrlList = Links.Select(x => new UrlVM
            {
                Id = x.Id,
                UrlLink = x.UrlLink??string.Empty,
                DomainLink = x.DomainLink ?? string.Empty,
                CustomerName=_db.CustomerTbls.Where(y=>y.Id==x.CustomerIdFk).Select(x=>x.CustomerName).FirstOrDefault(),
                CustomerPointName=x.CustomerPointName
            }).ToList();
            return View(UrlList);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> SaveLink()
        {
            var customer = await _customerRepository.ListOfCustomers();
            SaveUrlVM saveUrlVM = new SaveUrlVM();
            saveUrlVM.CustomerList= customer.Select(x => new CustomerInfo
            {
                CustomerId=x.Id,
                CustomerName=x.CustomerName
            }).ToList();
            return PartialView("~/Views/PartialViews/_AddUrlModal.cshtml", saveUrlVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SaveUrlVM>> SaveLink(SaveUrlVM urlVM)
        {
            bool isLinkExistForCustomer= await urlRepositoryObj.IsPointExistForCustomer(urlVM.CustomerPointName??String.Empty, urlVM.CustomerId);
            if (isLinkExistForCustomer)
                return Json(-1);
            bool isLinkExist = await urlRepositoryObj.IsLinkExist(domainLink + urlVM.UrlLink);
            if (isLinkExist == true)
            {
                return Json(0);
            }
            else
            {
                UrlTbl urlTbl = new UrlTbl()
                {
                    UrlLink = domainLink + urlVM.UrlLink,
                    DomainLink = urlVM.DomainLink,
                    CustomerIdFk= urlVM.CustomerId,
                    CustomerPointName = urlVM.CustomerPointName
                };
                urlRepositoryObj.SaveLink(urlTbl);
                return Json(1);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<SaveUrlVM>> UpdateLink(int id)
        {
            var obj =await urlRepositoryObj.GetUrlById(id);
            var customer = await _customerRepository.ListOfCustomers();
            SaveUrlVM editUrlVM = new SaveUrlVM();
            editUrlVM.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            editUrlVM.Id = obj.Id;
            editUrlVM.UrlLink = obj.UrlLink;
            editUrlVM.DomainLink = obj.DomainLink;
            editUrlVM.CustomerPointName = obj.CustomerPointName;
            editUrlVM.CustomerId = obj.CustomerIdFk??0;
            return PartialView("~/Views/PartialViews/_EditUrlModal.cshtml", editUrlVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateLink(SaveUrlVM urlVM)
        {
            bool isLinkExistForCustomerOnEdit = await urlRepositoryObj.IsPointExistForCustomerOnEdit(urlVM.CustomerPointName ?? String.Empty, urlVM.CustomerId, urlVM.Id);
            if (isLinkExistForCustomerOnEdit)
                return Json(-1);
            bool isLinkExist = await urlRepositoryObj.IsLinkExistOnEdit(domainLink + urlVM.UrlLink, urlVM.Id);
            if (isLinkExist == true)
            {
                return Json(0);
            }
            else
            {
                UrlTbl urlTbl = new UrlTbl()
                {
                    Id = urlVM.Id,
                    UrlLink = domainLink + urlVM.UrlLink,
                    DomainLink = urlVM.DomainLink,
                    CustomerIdFk = urlVM.CustomerId,
                    CustomerPointName = urlVM.CustomerPointName
                };
                urlRepositoryObj.UpdateLink(urlTbl);
                return Json(1);
            }
        }
        [Authorize]
        public ActionResult DeletLink(int Id)
        {
            urlRepositoryObj.DeleteUrl(Id);
            return Json(1);
        }
        [AllowAnonymous]
        public IActionResult checkRawUrl()
        {
            var statusCode = HttpContext.Response.StatusCode;
            var feauter = Request.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var path = feauter?.OriginalPath.ToString().Remove(0,1);
            var url = domainLink + path;

            var domainLinkObj = _db.UrlTbls.Where(x => x.UrlLink == url.Trim()).Select(x => x.DomainLink).FirstOrDefault();
            if (domainLinkObj != null)
            {
                return Redirect(domainLinkObj);
            }
            return StatusCode(statusCode);
        }
    }
}
