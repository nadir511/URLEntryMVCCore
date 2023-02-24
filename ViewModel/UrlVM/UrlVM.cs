using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.UrlVM
{
    public class UrlVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "URL")]
        public string? UrlLink { get; set; }
        [Required]
        [Display(Name = "Domain Link")]
        public string? DomainLink { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPointName { get; set; }

    }

}
