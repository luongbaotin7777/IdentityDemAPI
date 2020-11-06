using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Entities
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
