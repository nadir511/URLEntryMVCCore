using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Data;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.ViewModels;

namespace URLEntryMVC.Controllers
{
    public class URL : Controller
    {
        private readonly IUrlRepository urlRepositoryObj;
        private readonly DataContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private string domainLink = "http://tapthat.online/";

        public URL(IUrlRepository urlRepository,DataContext db, IHttpContextAccessor contextAccessor)
        {
            
            urlRepositoryObj = urlRepository;
            _db = db;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<UrlVM>>> ListOfLinks()
        {
            var Links = await urlRepositoryObj.ListOfLinks();

            List<UrlVM> UrlList = Links.Select(x => new UrlVM
            {
                Id = x.Id,
                UrlLink = x.UrlLink,
                DomainLink = x.DomainLink
            }).ToList();
            return View(UrlList);
        }
        [HttpGet]
        public ActionResult SaveLink()
        {
            SaveUrlVM saveUrlVM = new SaveUrlVM();
            return PartialView("~/Views/PartialViews/_AddUrlModal.cshtml", saveUrlVM);
        }
        [HttpPost]
        public async Task<ActionResult<SaveUrlVM>> SaveLink(SaveUrlVM urlVM)
        {
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
                };
                urlRepositoryObj.SaveLink(urlTbl);
                return Json(1);
            }
        }
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
        public ActionResult DeletLink(int Id)
        {
            urlRepositoryObj.DeleteUrl(Id);
            return Json(1);
        }
        public IActionResult checkRawUrl()
        {
            var statusCode = HttpContext.Response.StatusCode;
            //ViewData["statusCode"] = statusCode;
            var feauter = Request.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var path = feauter?.OriginalPath.ToString().Remove(0,1);
            var url = domainLink + path;

            var domainLinkObj = _db.UrlTbl.Where(x => x.UrlLink == url.Trim()).Select(x => x.DomainLink).FirstOrDefault();
            if (domainLinkObj != null)
            {
                return Redirect(domainLinkObj);
            }
            return StatusCode(statusCode);
        }
    }
}
