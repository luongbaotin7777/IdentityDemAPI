using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class UserReponse
    {
        public Guid Id { get; set; }
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
        public IList<string> Roles { get; set; }
    }
}
