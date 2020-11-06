using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController (IUserService userService)
        {
            _userService = userService;
        }
        //POST: /api/auth/register
        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register ([FromBody] UserRegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(request);
                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
           
        }
        //Put: /api/auth/grantrole/userId
        //[HttpPut("grantrole/{Id}")]
        //public async Task<IActionResult> RoleAssign ( Guid Id,[FromBody]RoleAssignRequest request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _userService.RoleAssign(Id, request);
        //        if (result.IsSuccess)
        //        {
        //            return Ok(result);
        //        }
        //        return BadRequest();
        //    }
        //    return BadRequest();
        //}
        //Post: /api/auth/registeradmin
        //[HttpPost("RegisterAdmin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] UserRegisterRequest request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _userService.RegisterAdminAsync(request);
        //        if (result.IsSuccess)
        //            return Ok(result);
        //        return BadRequest(result);
        //    }
        //    return BadRequest("Some properties are not valid");
        //}
        //POST: /api/auth/login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login ([FromBody] UserLoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
            
        }
       
        [HttpGet]
        public IActionResult Lockout()
        {
            return BadRequest("Your account is lock");
        }
        
        //Get: /api/auth/user
        [HttpGet("User")]
        [Authorize(Roles="Mod")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> GetAllUser(string Username,string Email)
        {
            var user = await _userService.GetAllUserAsync(Username, Email);
            if(user == null)
            {
                return NotFound("User not Found");
            }
            return Ok(user);
        }
        //Get: /api/auth/user/id
        [HttpGet("User/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string Id)
        {
            var user = await _userService.GetUserByIdAsync(Id);
            if(user == null)
            {
                return NotFound(user);
            }
            return Ok(user);
        }
        //DELETE: /api/auth/user/id
        [HttpDelete("User/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _userService.DeleteUserAsync(Id);
            if (user == null)
            {
                return NotFound(user);
            }
            else
            {
                return Ok(user);
            }
           
        }
        //Put: /api/auth/user/id
        [HttpPut("User/{Id}")]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> UpdateUser(Guid Id,UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userService.UpdateUserAsync(Id,request);
            if (user==null)
            {
                return BadRequest(user);
            }
            return Ok(user);
        }
       


    }
}
