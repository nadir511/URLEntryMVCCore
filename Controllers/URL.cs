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
using URLEntryMVC.ViewModel.CustomerVM;
using Microsoft.VisualBasic;
using URLEntryMVC.ViewModel.BusinessReviewVM;

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
            try
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
                else if (pointCategory == AppConstant.TapContractPoint)
                {
                    UrlList = UrlList.Where(x => x.PointCategoryId == AppConstant.TapThatContractPointId).ToList();
                }
                else if (pointCategory == AppConstant.BusinessRevPoint)
                {
                    UrlList = UrlList.Where(x => x.PointCategoryId == AppConstant.BusinessReviewPointId).ToList();
                }
                if (User.IsInRole(AppConstant.CustomerRole))
                {
                    var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
                    UrlList = UrlList.Where(x => x.CustomerId == userInfo.CustomerIdFk).ToList();
                    ViewBag.TotalPoints = UrlList.Count();
                }
                return View(UrlList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> SaveLink()
        {
            try
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
                saveUrlVM.businessReviewPoints = new List<BusinessReviewPoints>();
                return PartialView("~/Views/PartialViews/_AddUrlModal.cshtml", saveUrlVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SaveUrlVM>> SaveLink(SaveUrlVM urlVM)
        {
            try
            {
                var customerInfo = await _customerRepository.GetCustomerById(urlVM.CustomerId);
                bool isLinkExistForCustomer = await urlRepositoryObj.IsPointExistForCustomer(urlVM.CustomerPointName ?? String.Empty, urlVM.CustomerId);
                if (isLinkExistForCustomer)
                    return Json(-1);
                bool isPointMNExistCustomer = await urlRepositoryObj.IsManagementNameExistForCustomer(urlVM.ManagementName ?? String.Empty, urlVM.CustomerId);
                if (isPointMNExistCustomer)
                    return Json(-2);
                else
                {
                    SaveUrlVM urlTbl = new SaveUrlVM()
                    {
                        UrlLink = domainLink + '_' + customerInfo.CustomerName + '/' + urlVM.CustomerPointName,
                        DomainLink = urlVM.DomainLink,
                        CustomerId = urlVM.CustomerId,
                        CustomerPointName = urlVM.CustomerPointName,
                        ManagementName = urlVM.ManagementName,
                        PointCategoryId = urlVM.PointCategoryId,
                        Subject = urlVM.Subject,
                        Text = urlVM.Text,
                        SaveInLibrary = urlVM.SaveInLibrary,
                        businessReviewPoints=urlVM.businessReviewPoints,
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<SaveUrlVM>> UpdateLink(int id)
        {
            try
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
                editUrlVM.ManagementName = obj.ManagementName;
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
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateLink(SaveUrlVM urlVM)
        {
            try
            {
                var customerInfo = await _customerRepository.GetCustomerById(urlVM.CustomerId);
                bool isLinkExistForCustomerOnEdit = await urlRepositoryObj.IsPointExistForCustomerOnEdit(urlVM.CustomerPointName ?? String.Empty, urlVM.CustomerId, urlVM.Id);
                if (isLinkExistForCustomerOnEdit)
                    return Json(-1);
                bool isPointMNExistCustomer = await urlRepositoryObj.IsManagementNameExistOnEditForCustomer(urlVM.ManagementName ?? String.Empty, urlVM.CustomerId, urlVM.Id);
                if (isPointMNExistCustomer)
                    return Json(-2);
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
                        ManagementName = urlVM.ManagementName,
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
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<ActionResult> UpdateMultiPointInfo(List<string> pointIds, string pointCategory)
        {
            try
            {
                SaveUrlVM multiEditUrlVM = new SaveUrlVM();
                var customer = await _customerRepository.ListOfCustomers();
                multiEditUrlVM.CustomerList = customer.Select(x => new CustomerInfo
                {
                    CustomerId = x.Id,
                    CustomerName = x.CustomerName
                }).ToList();
                if (User.IsInRole(AppConstant.CustomerRole))
                {
                    var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
                    multiEditUrlVM.CustomerId = userInfo.CustomerIdFk ?? 0;
                }
                if (pointCategory == AppConstant.StdContractPoint)
                {
                    multiEditUrlVM.PointCategoryId = AppConstant.StdContractPointId;
                }
                else if (pointCategory == AppConstant.EmailContractPoint)
                {
                    multiEditUrlVM.PointCategoryId = AppConstant.EmailContractPointId;
                }
                multiEditUrlVM.PointIds = pointIds;
                multiEditUrlVM.EditType = AppConstant.MultiEdit;
                return PartialView("~/Views/PartialViews/_EditDomain.cshtml", multiEditUrlVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                            domainLinkObj.EditType = AppConstant.MultiEdit;
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
            try
            {
                var userInfo = await _userManager.FindByNameAsync(User.Identity.Name);
                var customerInfo = await _customerRepository.GetCustomerById(userInfo.CustomerIdFk ?? 0);
                var obj = await urlRepositoryObj.GetUrlById(id);
                SaveUrlVM editUrlVM = new SaveUrlVM();
                editUrlVM.CustomerId = userInfo.CustomerIdFk ?? 0;
                editUrlVM.CustomerList = new List<CustomerInfo>();
                editUrlVM.CustomerList.Add(new CustomerInfo { CustomerId = customerInfo.Id, CustomerName = customerInfo.CustomerName });
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
            catch (Exception ex)
            {
                throw ex;
            }
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
            try
            {
                urlRepositoryObj.DeleteUrl(Id);
                return Json(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionResult> ListOfSavePoints(int pointCategory,int customerId)
        {
            try
            {
                var Links = await urlRepositoryObj.ListOfLinks();
                var savePoints = Links.Where(x => x.SaveInLibrary == true && x.CategoryId == pointCategory && x.CustomerId == customerId).Select(x => new
                {
                    pointId = x.PointId,
                    pointName = x.PointManagementName
                }).ToList();
                return Json(new SelectList(savePoints, "pointId", "pointName"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionResult> ListOfSLinksByCustomer(int customerId)
        {
            try
            {
                var profiles = await _customerRepository.ListOfSocialProfByCustomer(customerId);
                var profilesObj = profiles.Select(x => new { x.text, x.value }).ToList();
                return Json(new SelectList(profilesObj, "value", "text"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionResult> InfoOfSavePoints(int savePointId)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<ActionResult> ListOfPrPointsByCustomerId(int customerId)
        {
            try
            {
                SaveUrlVM saveUrlVM = new SaveUrlVM();
                saveUrlVM.businessReviewPoints = await urlRepositoryObj.GetListOfBrPointsByCustomerId(customerId);
                return PartialView("~/Views/PartialViews/_BusinessPoints.cshtml", saveUrlVM);
            }
            catch (Exception)
            {

                throw;
            }
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
                if (domainLinkObj.PointCategoryIdFk == AppConstant.StdContractPointId || domainLinkObj.PointCategoryIdFk == AppConstant.TapThatContractPointId)
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
                else if (domainLinkObj.PointCategoryIdFk == AppConstant.BusinessReviewPointId)
                {
                    var BrPointUrl=_db.BusinessReviewPoints.Where(x=>x.UrlIdFk== domainLinkObj.Id && x.IsCurrentlyActive==true).FirstOrDefault();
                    if (BrPointUrl!=null)
                    {
                        return Redirect(BrPointUrl.PointUrl ?? "");
                    }
                }
            }
            return StatusCode(statusCode);
        }
    }
}
