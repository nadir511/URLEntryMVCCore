using System.ComponentModel.DataAnnotations;
using URLEntryMVC.ViewModels;

namespace URLEntryMVC.ViewModel
{
    public class RegisterViewModel
    {
        public int UserId { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
        public string? ConfirmPassword { get; set; }
        public List<CustomerInfo>? CustomerList { get; set; }
    }
}
