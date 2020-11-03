using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityDemo.API.Dtos
{
    public class UserRegisterRequest
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
