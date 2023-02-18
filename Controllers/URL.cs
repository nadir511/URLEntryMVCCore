using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModels;
using URLEntryMVC.Data;

namespace URLEntryMVC.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

    public class URL : Controller
    {
        private readonly IUrlRepository urlRepositoryObj;
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;
        private string domainLink = "http://tapthat.online/_";

        public URL(IUrlRepository urlRepository, DataContext db, IHttpContextAccessor contextAccessor,ICustomerRepository customerRepository)
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

            List<UrlVM> UrlList = Links.Select(x => new UrlVM
            {
                Id = x.Id,
                UrlLink = x.UrlLink??string.Empty,
                DomainLink = x.DomainLink ?? string.Empty
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
            bool isLinkExistForCustomer= await urlRepositoryObj.IsLinkExist(urlVM.UrlLinkForCustomer??String.Empty);
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
                    UrlLinkForCustomer= urlVM.UrlLinkForCustomer
                };
                urlRepositoryObj.SaveLink(urlTbl);
                return Json(1);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UrlVM>> UpdateLink(int id)
        {
            var obj =await urlRepositoryObj.GetUrlById(id);
            UrlVM EditObj = new UrlVM();
            EditObj.Id = obj.Id;
            EditObj.UrlLink = obj.UrlLink;
            EditObj.DomainLink = obj.DomainLink;
            return PartialView("~/Views/PartialViews/_EditUrlModal.cshtml", EditObj);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UrlVM>> UpdateLink(UrlVM urlVM)
        {
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
