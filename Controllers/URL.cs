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
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using System.Net.Mime;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Utilities;

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
            ViewBag.TotalPoints = Links.Count();
            ViewBag.TotalCustomers = _db.CustomerTbls.ToList().Count();
            ViewBag.pointCategory = pointCategory;

            List<UrlVM> UrlList = Links.Select(x => new UrlVM
            {
                Id = x.PointId,
                PointCategoryId = x.CategoryId,
                PointCategoryName = x.CategoryName,
                UrlLink = x.PointLink,
                DomainLink = x.DomainLink ?? string.Empty,
                CustomerName = x.CustomerName,
                CustomerId = x.CustomerId,
                CustomerPointName = x.PointName,
                CustomerNotes = x.CustomerNotes,
                TotalClicks = x.TotalCliks,
                Subject = x.Esubject,
                Body = x.Body,
                PointEmails = x.Emails
            }).ToList();
            if (pointCategory == AppConstant.StdContractPoint)
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
                    Text = urlVM.Text,
                    SaveInLibrary = urlVM.SaveInLibrary
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
            editUrlVM.SaveInLibrary = obj.SaveInLibrary ?? false;
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
                    SaveInLibrary = urlVM.SaveInLibrary,
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
        [HttpGet]
        public ActionResult UpdateMultiPointInfo(List<string> pointIds, string pointCategory)
        {
            SaveUrlVM multiEditUrlVM = new SaveUrlVM();
            if (pointCategory == AppConstant.StdContractPoint)
            {
                multiEditUrlVM.PointCategoryId = AppConstant.StdContractPointId;
            }
            else if (pointCategory == AppConstant.EmailContractPoint)
            {
                multiEditUrlVM.PointCategoryId = AppConstant.EmailContractPointId;
            }
            multiEditUrlVM.PointIds = pointIds;
            multiEditUrlVM.EditType = "MultiEdit";
            return PartialView("~/Views/PartialViews/_EditDomain.cshtml", multiEditUrlVM);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateMultiPointInfo(SaveUrlVM urlVM)
        {
            try
            {
                if (urlVM.PointIds!=null)
                {
                    foreach (var item in urlVM.PointIds)
                    {
                        int pointId =Convert.ToInt32(item);
                        SaveUrlVM? domainLinkObj = await _db.UrlTbls.Where(x => x.Id == pointId).Select(x => new SaveUrlVM
                        {
                            Id = x.Id,
                            DomainLink = urlVM.DomainLink ?? x.DomainLink,
                            UrlLink = x.UrlLink,
                            CustomerId = x.CustomerIdFk ?? 0,
                            CustomerPointName = x.CustomerPointName,
                            PointCategoryId = x.PointCategoryIdFk,
                            CustomerNotes = urlVM.CustomerNotes,
                            Subject = urlVM.Subject ?? x.Subject,
                            Text = urlVM.Text ?? x.Body,
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
                    }
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                throw ex;
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
                    DomainLink = urlVM.DomainLink ?? x.DomainLink,
                    UrlLink = x.UrlLink,
                    CustomerId = x.CustomerIdFk ?? 0,
                    CustomerPointName = x.CustomerPointName,
                    PointCategoryId = x.PointCategoryIdFk,
                    CustomerNotes = urlVM.CustomerNotes,
                    Subject = urlVM.Subject ?? x.Subject,
                    Text = urlVM.Text ?? x.Body,
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
        public async Task<ActionResult> ListOfSavePoints(int pointCategory)
        {
            var Links = await urlRepositoryObj.ListOfLinks();
            var savePoints = Links.Where(x => x.SaveInLibrary == true && x.CategoryId == pointCategory).Select(x => new
            {
                pointId = x.PointId,
                pointName = x.PointName
            }).ToList();
            return Json(new SelectList(savePoints, "pointId", "pointName"));
        }
        public async Task<ActionResult> InfoOfSavePoints(int savePointId)
        {
            var obj = await urlRepositoryObj.GetUrlById(savePointId);
            SaveUrlVM editUrlVM = new SaveUrlVM();
            editUrlVM.Id = obj.Id;
            editUrlVM.DomainLink = obj.DomainLink;
            editUrlVM.CustomerNotes = obj.CustomerNotes;
            editUrlVM.PointCategoryId = obj.PointCategoryIdFk;
            editUrlVM.Subject = obj.Subject;
            editUrlVM.Text = obj.Body;
            editUrlVM.PointCategoryId = obj.PointCategoryIdFk;

            string[] emails = new string[3];
            if (obj.PointCategoryIdFk == 2)
            {
                var emailsStr = await urlRepositoryObj.GetEmailsByPointId(savePointId);
                if (emailsStr != null)
                    emails = emailsStr.Split(',');
                editUrlVM.Email1 = emails[0];
                editUrlVM.Email2 = emails.ElementAtOrDefault(1) != null ? emails[1] : null;
                editUrlVM.Email3 = emails.ElementAtOrDefault(2) != null ? emails[2] : null;
            }
            return Json(editUrlVM);
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
                if (domainLinkObj.PointCategoryIdFk == AppConstant.StdContractPointId)
                {
                    return Redirect(domainLinkObj.DomainLink ?? "");
                }
                else if (domainLinkObj.PointCategoryIdFk == AppConstant.EmailContractPointId)
                {
                    var emails = _db.PointEmails.Where(x => x.PointIdFk == domainLinkObj.Id).Select(x => x.Email).FirstOrDefault();
                    var encodedSubject = Uri.EscapeDataString(domainLinkObj.Subject ?? "");
                    var encodedBody = Uri.EscapeDataString(domainLinkObj.Body ?? "");

                    //string encodedSubjectText = "=?utf-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(domainLinkObj.Subject ?? "")) + "?=";
                    //string encodedBodyText = "=?utf-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(domainLinkObj.Body ?? "")) + "?=";

                    var mailToStr = "mailto:" + emails + "?subject=" + HttpUtility.HtmlAttributeEncode(encodedSubject) + "&body=" + HttpUtility.HtmlAttributeEncode(encodedBody);

                    //string mailtoLink = string.Format("mailto:{0}?Content-Type={1}&subject={2}&body={3}", emails, "text/plain; charset=utf-8", HttpUtility.UrlEncode(encodedSubjectText), HttpUtility.UrlEncode(encodedBodyText));
                    //string mailtoLink = "mailto:recipient@example.com?subject=Example%20Subject&body=%3D%3Futf-8%3FB%3F44GT44KT44Gr44Gh44Gv%3F%3D%0D%0AContent-Type%3A%20text%2Fplain%3B%20charset%3Dutf-8";
                    return Redirect(mailToStr);
                }
            }
            return StatusCode(statusCode);
        }
    }
}
