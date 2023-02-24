﻿using System.ComponentModel.DataAnnotations;
using URLEntryMVC.ViewModel.UrlVM;

namespace URLEntryMVC.ViewModel.AccountVM
{
    public class EditRegisterViewModel
    {
        public string? UserId { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public UsersVM? UsersInfo { get; set; }
        public List<CustomerInfo>? CustomerList { get; set; }
        public int? CustomerId { get; set; }
        public List<CreateRoleViewModel>? RolesList { get; set; }
        public string? RoleName { get; set; }
    }
}
