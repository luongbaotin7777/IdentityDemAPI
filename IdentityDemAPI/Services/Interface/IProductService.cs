using IdentityDemo.API.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityDemo.API.Services.Interface
{
    public interface IProductService
    {
        Task<int> CreateProduct(ProductRequest request);
        Task<List<ProductReponse>> GetAllProduct(string Name, string Price);
        Task<ProductReponse> GetProductById(int ProductId);
        Task<int> UpdateProduct(ProductRequest request);
        Task<int> DeleteProduct(int Id);
    }
}
