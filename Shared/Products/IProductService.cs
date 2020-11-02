
using Shared.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Products
{
    public interface IProductService
    {
         Task<int> CreateProduct(ProductRequest request);
         Task<List<ProductReponse>> GetAllProduct();
         Task<ProductReponse> GetProductById(int CategoryId);
         Task<int> UpdateProduct(ProductRequest request);
         Task<int> DeleteProduct(int Id);
    }
}
