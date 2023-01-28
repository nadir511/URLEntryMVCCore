using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModels
{
    public class UrlVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="URL")]
        public string UrlLink { get; set; }
        [Required]
        [Display(Name = "Domain Link")]
        public string DomainLink { get; set; }
    }
}
