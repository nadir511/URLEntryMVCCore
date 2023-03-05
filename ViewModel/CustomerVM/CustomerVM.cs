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

        public IFormFile? CustomerLogo { get; set; }
        public byte[] CustomerPic { get; set; } = null!;
    }
    public class DeleteCustomerVM
    {
        public List<UrlVM.UrlVM>? customerPoint { get; set; }
        public List<UsersVM>? customerUsers { get; set; }
        public bool ? isDeleted { get; set; }   
    }
}
