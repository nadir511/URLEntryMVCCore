using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModels
{
    public class SaveUrlVM
    {
        [Required]
        [Display(Name = "URL Link")]
        public string? UrlLink { get; set; }
        public string? UrlLinkForCustomer { get; set; }
        [Required]
        [Display(Name = "Domain Link")]
        public string? DomainLink { get; set; }
        public int CustomerId { get; set; }
        public List<CustomerInfo>? CustomerList { get; set; }
    }
    public class CustomerInfo
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
}
