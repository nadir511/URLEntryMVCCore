using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class CreateRoleViewModel
    {
        public string? RoleId { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string? RoleName { get; set; }
    }
}
