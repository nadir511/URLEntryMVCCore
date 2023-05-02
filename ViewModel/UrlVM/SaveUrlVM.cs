using System.ComponentModel.DataAnnotations;
using URLEntryMVC.ViewModel.PointCategoryVM;

namespace URLEntryMVC.ViewModel.UrlVM
{
    public class SaveUrlVM
    {
        public int Id { get; set; }
        public List<string>? PointIds { get; set; }
        //[Required]
        [Display(Name = "URL Link")]
        public string? UrlLink { get; set; }
        public string? CustomerPointName { get; set; }
        [Required]
        [Display(Name = "Management Name")]
        public string? ManagementName { get; set; }
        [Display(Name = "Domain Link")]
        public string? DomainLink { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public int? PointCategoryId { get; set; }
        public string? CustomerNotes { get; set; }
        [Required]
        [Display(Name = "Subject")]
        public string? Subject { get; set; }
        [Required]
        [Display(Name = "Text")]
        public string? Text { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email1 { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email2 { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email3 { get; set; }
        public string? allEmailsStr { get; set; }
        public bool SaveInLibrary { get; set; }
        public List<CustomerInfo>? CustomerList { get; set; }
        public List<PointCategoryInfo>? PointCategoryList {get;set;}
        public List<LibraryListPoints>? libraryListPoints { get; set; }
        public string? EditType { get; set; }
    }
    public class CustomerInfo
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
    public class LibraryListPoints
    {
        public int PointId { get; set; }
        public string? PointName { get; set; }
    }
}
