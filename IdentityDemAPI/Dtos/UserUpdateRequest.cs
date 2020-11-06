using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class UserUpdateRequest
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
  
        public DateTime? Dob { get; set; }
       
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleId { get;set; }

    }
}
