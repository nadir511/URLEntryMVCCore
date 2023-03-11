using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
