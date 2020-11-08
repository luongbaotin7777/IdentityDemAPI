using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }
        [HttpPost]
        
        public async Task<IActionResult> Create([FromBody]ProductRequest request)
        {
            var product = await _service.CreateProduct(request);
            return Ok(product);
        }
        [HttpGet]
        [Authorize(Permission.View)]
        public async Task<IActionResult> GetAll(string Name, string Price)
        {
            var product = await _service.GetAllProduct(Name,Price);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var product = await _service.GetProductById(Id);
            if(product == null)
            {
                return NotFound($"Id not Found. Please re-enter the corret Id");
            }
            return Ok(product);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]ProductRequest request)
        {
            
            var product = await _service.UpdateProduct(request);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _service.DeleteProduct(Id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok();
        }
       

    }
}
