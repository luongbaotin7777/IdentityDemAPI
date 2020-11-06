using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Dtos
{
    public class SelectRoleItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
