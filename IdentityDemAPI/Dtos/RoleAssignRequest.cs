using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class RoleAssignRequest
    {
        public SelectRoleItem[] SelectedRoles { get; set; }
    }
}
