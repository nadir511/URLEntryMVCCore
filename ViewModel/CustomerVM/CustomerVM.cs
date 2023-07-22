using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using URLEntryMVC.Entities;
using URLEntryMVC.ViewModel.AccountVM;

namespace URLEntryMVC.ViewModel.CustomerVM
{
    public class CustomerVM
    {
        public int Id { get; set; }
        [Display(Name ="Customer Name")]
        [Required]
        [RegularExpression(@"^[^\s\,]*$", ErrorMessage = "No white space allowed")]
        public string CustomerName { get; set; } = null!;
        [Display(Name = "Address")]
        [Required]
        public string? Address { get; set; }

        public string? ContactNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public bool isProfileDisabled { get; set; }
        public IFormFile? CustomerLogo { get; set; }
        public byte[] CustomerPic { get; set; } = null!;
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? LinkedIn { get; set; }
        public string? TikTok { get; set; }
        public string? Youtube { get; set; }
        public string? Snapchat { get; set; }
        public List<BusinessReviewUrl>? businessReviewUrls { get; set; }
    }
    public class DeleteCustomerVM
    {
        public List<UrlVM.UrlVM>? customerPoint { get; set; }
        public List<UsersVM>? customerUsers { get; set; }
        public bool ? isDeleted { get; set; }   
    }
    public class BusinessReviewUrl
    {
        public string? UrlName { get; set; }
        public int? BusinessPointId { get; set; }
        [Display(Name = "Point URL")]
        [DataType(DataType.Url)]
        public string? PointUrl { get; set; }
    }
}
