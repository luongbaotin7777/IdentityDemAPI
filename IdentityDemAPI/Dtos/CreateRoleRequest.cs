﻿using IdentityDemo.API.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class CreateRoleRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        
    }
}
