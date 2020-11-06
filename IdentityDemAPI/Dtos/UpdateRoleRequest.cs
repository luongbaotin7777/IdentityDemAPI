using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class UpdateRoleRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Users { get; set; } 
    }
}
