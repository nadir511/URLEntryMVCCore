using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class ResetPasswordVM
    {
        public string? Id { get; set; }
        
        [Required]
        [StringLength(20, ErrorMessage = "Maximum length allowed for new password is 20 characters.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }

    }
}
