using System.ComponentModel.DataAnnotations;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class RegisterViewModel
    {
        public string? UserId { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
        public string? ConfirmPassword { get; set; }
        public UsersVM? UsersInfo { get; set; }
        public List<CustomerInfo>? CustomerList { get; set; }
        public int? CustomerId { get; set; }
        public List<CreateRoleViewModel>? RolesList { get; set; }
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
