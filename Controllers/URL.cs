using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using URLEntryMVC.Entities;
using URLEntryMVC.Interfaces;
using URLEntryMVC.Data;
using URLEntryMVC.ViewModel.UrlVM;
using Microsoft.EntityFrameworkCore;
using URLEntryMVC.Extensions;
using Microsoft.AspNetCore.Identity;
using URLEntryMVC.ApplicationConstants;
using URLEntryMVC.ViewModel.PointCategoryVM;
using System.Text;

namespace URLEntryMVC.Controllers
{

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

    public class URL : Controller
    {
        private readonly IUrlRepository urlRepositoryObj;
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _db;
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<ApplicationUserExtension> _userManager;
        private string domainLink = "https://tapthat.online/";

        public URL(IUrlRepository urlRepository, IWebHostEnvironment environment, DataContext db, ICustomerRepository customerRepository, UserManager<ApplicationUserExtension> userManager)
        {
            urlRepositoryObj = urlRepository;
            _env = environment;
            _db = db;
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UrlVM>>> ListOfLinks(string pointCategory)
        {
            var Links = await urlRepositoryObj.ListOfLinks();
            ViewBag.TotalPoints = _db.UrlTbls.ToList().Count();
            ViewBag.TotalCustomers = _db.CustomerTbls.ToList().Count();
            ViewBag.pointCategory = pointCategory;

            List<UrlVM> UrlList = Links.Select(x => new UrlVM
            {
                Id = x.Id,
                PointCategoryId = x.PointCategoryIdFk,
                PointCategoryName = x.PointCategoryIdFk == 1 ? AppConstant.StdContractPoint : AppConstant.EmailContractPoint,
                UrlLink = x.UrlLink ?? string.Empty,
                DomainLink = x.DomainLink ?? string.Empty,
                CustomerName = _db.CustomerTbls.Where(y => y.Id == x.CustomerIdFk).Select(x => x.CustomerName).FirstOrDefault(),
                CustomerId = x.CustomerIdFk,
                CustomerPointName = x.CustomerPointName,
                CustomerNotes = x.CustomerNotes,
                TotalClicks = x.TotalClicks,
                Subject = x.Subject,
                Body = x.Body,
                PointEmails = _db.PointEmails.Where(y => y.PointIdFk == x.Id).Select(x => x.Email).FirstOrDefault()
            }).ToList();
            if (pointCategory==AppConstant.StdContractPoint)
            {
                UrlList = UrlList.Where(x => x.PointCategoryId == AppConstant.StdContractPointId).ToList();
            }
            else if (pointCategory == AppConstant.EmailContractPoint)
            {
                UrlList = UrlList.Where(x => x.PointCategoryId == AppConstant.EmailContractPointId).ToList();
            }
            if (User.IsInRole(AppConstant.CustomerRole))
            {
                var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
                UrlList = UrlList.Where(x => x.CustomerId == userInfo.CustomerIdFk).ToList();
                ViewBag.TotalPoints = UrlList.Count();
            }
            return View(UrlList);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> SaveLink()
        {
            var customer = await _customerRepository.ListOfCustomers();
            var pointCategory = await _customerRepository.ListOfPointCategories();
            SaveUrlVM saveUrlVM = new SaveUrlVM();
            saveUrlVM.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            saveUrlVM.PointCategoryList = pointCategory.Select(x => new PointCategoryInfo
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            }).ToList();
            return PartialView("~/Views/PartialViews/_AddUrlModal.cshtml", saveUrlVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SaveUrlVM>> SaveLink(SaveUrlVM urlVM)
        {
            var customerInfo = await _customerRepository.GetCustomerById(urlVM.CustomerId);
            bool isLinkExistForCustomer = await urlRepositoryObj.IsPointExistForCustomer(urlVM.CustomerPointName ?? String.Empty, urlVM.CustomerId);
            if (isLinkExistForCustomer)
                return Json(-1);
            else
            {
                SaveUrlVM urlTbl = new SaveUrlVM()
                {
                    UrlLink = domainLink + '_' + customerInfo.CustomerName + '/' + urlVM.CustomerPointName,
                    DomainLink = urlVM.DomainLink,
                    CustomerId = urlVM.CustomerId,
                    CustomerPointName = urlVM.CustomerPointName,
                    PointCategoryId = urlVM.PointCategoryId,
                    Subject = urlVM.Subject,
                    Text = urlVM.Text
                };
                StringBuilder? emails = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(urlVM.Email1))
                {
                    emails.Append(urlVM.Email1);
                }
                if (!string.IsNullOrWhiteSpace(urlVM.Email2))
                {
                    emails.Append("," + urlVM.Email2);
                }
                if (!string.IsNullOrWhiteSpace(urlVM.Email3))
                {
                    emails.Append("," + urlVM.Email3);
                }
                urlTbl.allEmailsStr = emails.ToString();
                urlRepositoryObj.SaveLink(urlTbl);
                return Json(1);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<SaveUrlVM>> UpdateLink(int id)
        {
            var obj = await urlRepositoryObj.GetUrlById(id);
            var customer = await _customerRepository.ListOfCustomers();
            var pointCategory = await _customerRepository.ListOfPointCategories();
            SaveUrlVM editUrlVM = new SaveUrlVM();
            editUrlVM.CustomerList = customer.Select(x => new CustomerInfo
            {
                CustomerId = x.Id,
                CustomerName = x.CustomerName
            }).ToList();
            editUrlVM.PointCategoryList = pointCategory.Select(x => new PointCategoryInfo
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            }).ToList();
            editUrlVM.Id = obj.Id;
            editUrlVM.UrlLink = obj.UrlLink;
            editUrlVM.DomainLink = obj.DomainLink;
            editUrlVM.CustomerPointName = obj.CustomerPointName;
            editUrlVM.CustomerId = obj.CustomerIdFk ?? 0;
            editUrlVM.PointCategoryId = obj.PointCategoryIdFk;
            editUrlVM.Subject = obj.Subject;
            editUrlVM.Text = obj.Body;
            string[] emails = new string[3];

            if (obj.PointCategoryIdFk == AppConstant.EmailContractPointId)
            {
                var emailsStr = await urlRepositoryObj.GetEmailsByPointId(id);
                if (emailsStr != null)
                    emails = emailsStr.Split(',');
                editUrlVM.Email1 = emails[0];
                editUrlVM.Email2 = emails.ElementAtOrDefault(1) != null ? emails[1] : null;
                editUrlVM.Email3 = emails.ElementAtOrDefault(2) != null ? emails[2] : null;
            }

            return PartialView("~/Views/PartialViews/_EditUrlModal.cshtml", editUrlVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateLink(SaveUrlVM urlVM)
        {
            var customerInfo = await _customerRepository.GetCustomerById(urlVM.CustomerId);
            bool isLinkExistForCustomerOnEdit = await urlRepositoryObj.IsPointExistForCustomerOnEdit(urlVM.CustomerPointName ?? String.Empty, urlVM.CustomerId, urlVM.Id);
            if (isLinkExistForCustomerOnEdit)
                return Json(-1);
            else
            {
                SaveUrlVM urlTbl = new SaveUrlVM()
                {
                    Id = urlVM.Id,
                    UrlLink = domainLink + '_' + customerInfo.CustomerName + '/' + urlVM.CustomerPointName,
                    DomainLink = urlVM.DomainLink,
                    CustomerId = urlVM.CustomerId,
                    CustomerPointName = urlVM.CustomerPointName,
                    PointCategoryId = urlVM.PointCategoryId,
                    Subject = urlVM.Subject,
                    Text = urlVM.Text
                };
                if (urlVM.PointCategoryId == AppConstant.EmailContractPointId)
                {
                    StringBuilder? emails = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(urlVM.Email1))
                    {
                        emails.Append(urlVM.Email1);
                    }
                    if (!string.IsNullOrWhiteSpace(urlVM.Email2))
                    {
                        emails.Append("," + urlVM.Email2);
                    }
                    if (!string.IsNullOrWhiteSpace(urlVM.Email3))
                    {
                        emails.Append("," + urlVM.Email3);
                    }
                    urlTbl.allEmailsStr = emails.ToString();
                }
                urlRepositoryObj.UpdateLink(urlTbl);
                return Json(1);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> UpdatePointInfo(int id)
        {
            var obj = await urlRepositoryObj.GetUrlById(id);
            SaveUrlVM editUrlVM = new SaveUrlVM();
            editUrlVM.Id = obj.Id;
            editUrlVM.DomainLink = obj.DomainLink;
            editUrlVM.CustomerNotes = obj.CustomerNotes;
            editUrlVM.PointCategoryId = obj.PointCategoryIdFk;
            editUrlVM.Subject = obj.Subject;
            editUrlVM.Text = obj.Body;

            string[] emails = new string[3];
            if (obj.PointCategoryIdFk == 2)
            {
                var emailsStr = await urlRepositoryObj.GetEmailsByPointId(id);
                if (emailsStr != null)
                    emails = emailsStr.Split(',');
                editUrlVM.Email1 = emails[0];
                editUrlVM.Email2 = emails.ElementAtOrDefault(1) != null ? emails[1] : null;
                editUrlVM.Email3 = emails.ElementAtOrDefault(2) != null ? emails[2] : null;
            }
            return PartialView("~/Views/PartialViews/_EditDomain.cshtml", editUrlVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdatePointInfo(SaveUrlVM urlVM)
        {
            try
            {
                SaveUrlVM? domainLinkObj = await _db.UrlTbls.Where(x => x.Id == urlVM.Id).Select(x => new SaveUrlVM
                {
                    Id = x.Id,
                    DomainLink = urlVM.DomainLink??x.DomainLink,
                    UrlLink=x.UrlLink,
                    CustomerId=x.CustomerIdFk??0,
                    CustomerPointName=x.CustomerPointName,
                    PointCategoryId=x.PointCategoryIdFk,
                    CustomerNotes = urlVM.CustomerNotes,
                    Subject = urlVM.Subject??x.Subject,
                    Text = urlVM.Text??x.Body,
                }).FirstOrDefaultAsync();
                if (domainLinkObj != null)
                {
                    if (domainLinkObj.PointCategoryId == AppConstant.EmailContractPointId)
                    {
                        StringBuilder? emails = new StringBuilder();
                        if (!string.IsNullOrWhiteSpace(urlVM.Email1))
                        {
                            emails.Append(urlVM.Email1);
                        }
                        if (!string.IsNullOrWhiteSpace(urlVM.Email2))
                        {
                            emails.Append("," + urlVM.Email2);
                        }
                        if (!string.IsNullOrWhiteSpace(urlVM.Email3))
                        {
                            emails.Append("," + urlVM.Email3);
                        }
                        domainLinkObj.allEmailsStr = emails.ToString();
                    }
                    urlRepositoryObj.UpdateLink(domainLinkObj);
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        public ActionResult DeletLink(int Id)
        {
            urlRepositoryObj.DeleteUrl(Id);
            return Json(1);
        }
        [AllowAnonymous]
        public async Task<IActionResult> checkRawUrl()
        {
            var statusCode = HttpContext.Response.StatusCode;
            var feauter = Request.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var path = feauter?.OriginalPath.ToString().Remove(0, 1);
            var url = domainLink + path;

            var domainLinkObj = await _db.UrlTbls.Where(x => x.UrlLink == url.Trim()).FirstOrDefaultAsync();
            if (domainLinkObj != null)
            {
                domainLinkObj.TotalClicks = domainLinkObj.TotalClicks == null ? 1 : domainLinkObj.TotalClicks + 1;
                await _db.SaveChangesAsync();
                if (domainLinkObj.PointCategoryIdFk==AppConstant.StdContractPointId)
                {
                    return Redirect(domainLinkObj.DomainLink ?? "");
                }
                else if (domainLinkObj.PointCategoryIdFk == AppConstant.EmailContractPointId)
                {
                    var emails = _db.PointEmails.Where(x => x.PointIdFk == domainLinkObj.Id).Select(x => x.Email).FirstOrDefault();
                    return Redirect("mailto:"+emails+ "?subject=" + domainLinkObj .Subject+ "&body="+ domainLinkObj.Body);
                }
            }
            return StatusCode(statusCode);
        }
    }
}
