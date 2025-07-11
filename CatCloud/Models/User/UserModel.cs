﻿using System.ComponentModel.DataAnnotations;

namespace CatCloud.Models.User
{
    public class UserModel
    {
        [EmailAddress]
        public required string Email { get; set; }
        [StringLength(20, ErrorMessage = "{0} marimea trebuie sa fie intre {2} si {1}.", MinimumLength = 6)]
        public required string Username { get; set; }
        [StringLength(20, ErrorMessage = "{0} marimea trebuie sa fie intre {2} si {1}.", MinimumLength = 6)]
        public required string Password { get; set; }
        [Range(0, 50)]
        public double TotalStorage { get; set; }
        //public double AvailableStorage { get; set; }
    }
}
