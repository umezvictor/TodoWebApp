﻿using System.ComponentModel.DataAnnotations;

namespace Web.Client.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
