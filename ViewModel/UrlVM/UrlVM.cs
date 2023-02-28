﻿using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.UrlVM
{
    public class UrlVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Point")]
        public string? UrlLink { get; set; }
        [Required]
        [Display(Name = "Domain Link")]
        public string? DomainLink { get; set; }
        public string? CustomerName { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerPointName { get; set; }
        public string? CustomerNotes { get; set; }
        public int? TotalClicks { get; set; }

    }

}
