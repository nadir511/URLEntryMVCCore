﻿using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.CustomerVM
{
    public class CustomerVM
    {
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; } = null!;

        public string? Address { get; set; }

        public string? ContactNumber { get; set; }

        public IFormFile? CustomerLogo { get; set; }
        public byte[] CustomerPic { get; set; } = null!;
    }
}