using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModels
{
    public class SaveUrlVM
    {
        [Required]
        [Display(Name = "URL Link")]
        public string UrlLink { get; set; }
        [Required]
        [Display(Name = "Domain Link")]
        public string DomainLink { get; set; }
    }
}
