using System.ComponentModel.DataAnnotations;
using URLEntryMVC.ViewModel.AccountVM;

namespace URLEntryMVC.ViewModel.CustomerVM
{
    public class CustomerVM
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[^\s\,]*$", ErrorMessage = "No white space allowed")]
        public string CustomerName { get; set; } = null!;

        public string? Address { get; set; }

        public string? ContactNumber { get; set; }
        public string? CustomerEmail { get; set; }

        public IFormFile? CustomerLogo { get; set; }
        public byte[] CustomerPic { get; set; } = null!;
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? LinkedIn { get; set; }
        public string? TikTok { get; set; }
        public string? Youtube { get; set; }
        public string? Snapchat { get; set; }
    }
    public class DeleteCustomerVM
    {
        public List<UrlVM.UrlVM>? customerPoint { get; set; }
        public List<UsersVM>? customerUsers { get; set; }
        public bool ? isDeleted { get; set; }   
    }
}
