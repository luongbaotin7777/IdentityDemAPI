using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityDemoAPI.Data.Models
{
    public class AppRole:IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
