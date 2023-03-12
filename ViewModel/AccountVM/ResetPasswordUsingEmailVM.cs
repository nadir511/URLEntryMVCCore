﻿using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class ResetPasswordUsingEmailVM
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
