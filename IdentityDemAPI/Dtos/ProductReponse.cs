using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityDemo.API.Dtos
{
    public class ProductReponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        //public string CategoryName { get; set; }
    }
}
