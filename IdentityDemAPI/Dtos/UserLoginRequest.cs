using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityDemo.API.Dtos
{
    public class UserLoginRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6)]

        public string Password { get; set; }
        public bool Remeberme { get; set; }
    }
}
