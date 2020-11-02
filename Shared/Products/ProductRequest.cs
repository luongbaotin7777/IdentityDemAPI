using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Products
{
    public class ProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
