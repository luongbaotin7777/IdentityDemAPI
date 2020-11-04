using IdentityDemo.API.BaseRepository;
using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace IdentityDemo.API.Services.Handle
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateProduct(ProductRequest request)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description

            };
            await _context.Products.AddAsync(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteProduct(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            if (product == null) throw new Exception($"Id not found, Please re-enter the correct Id ");
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();

        }

        public async Task<List<ProductReponse>> GetAllProduct(string Name,string Price)
        {
            if(!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Price))
            {
                var product = _context.Products.Where(x => x.Name.Contains(Name) || x.Price.ToString().Contains(Price));
                var result = await product.Select(x => new ProductReponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description
                }).ToListAsync();
                return result;
            }
            else
            {
                var product = await _context.Products.Select(x => new ProductReponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description
                }).ToListAsync();
                return product;
            }
          

        }

        public async Task<ProductReponse> GetProductById(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            if (product == null) throw new Exception($"Cannot find a product with id: {Id}");
            var data = new ProductReponse()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
            return data;

        }

        public async Task<int> UpdateProduct(ProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            if (product == null) throw new Exception("Id not Found");
            product.Name = request.Name;
            product.Price = request.Price;
            product.Description = request.Description;
            _context.Products.Update(product);
            return await _context.SaveChangesAsync();
        }
    }
}
