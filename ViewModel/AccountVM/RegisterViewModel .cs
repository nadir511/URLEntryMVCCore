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
        [StringLength(20, ErrorMessage = "Maximum length allowed for new password is 20 characters.", MinimumLength = 8)]
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
