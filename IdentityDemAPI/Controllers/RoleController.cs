using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.API.Dtos;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        //Post: api/role/create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.CreateRole(request);
            if (role.IsSuccess)
            {
                return Ok(role);
            }
            return BadRequest(role);
        }
        //GET: api/role/
        [HttpGet]
        public async Task<IActionResult> GetAllRole(string Name)
        {
            var role = await _roleService.GetAllRole(Name);
            if (role == null)
            {
                return NotFound("Role is not Found");
            }
            return Ok(role);
        }
        //Get api/role/roleid
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetRoleById(string Id)
        {
            var role = await _roleService.GetRoleById(Id.ToString());
            if (role == null)
            {
                return NotFound("Id not Found");
            }
            return Ok(role);
        }
        //PUT api/role/roleid
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateRole(Guid Id, UpdateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = await _roleService.UpdateRole(Id, request);
            if (role == null)
            {
                return BadRequest(role);

            }
            return Ok(role);

        }
        //Delete /api/role/id
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteRole(Guid Id)
        {
            var role = await _roleService.DeleteRole(Id);
            if (role == null)
            {
                return NotFound(role);
            };
            return Ok(role);
        }
       
       
    }
}
