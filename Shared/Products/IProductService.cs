
using Shared.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Products
{
    public interface IProductService
    {
        public Task<int> CreateProduct(ProductRequest request);
        public Task<List<ProductReponse>> GetAllProduct();
        public Task<ProductReponse> GetProductById(int CategoryId);
        public Task<int> UpdateProduct(ProductRequest request);
        public Task<int> DeleteProduct(int Id);
    }
}
