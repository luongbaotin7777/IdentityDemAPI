using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Users;

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
        //api/auth/register
        [HttpPost("Register")]
        [AllowAnonymous]
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
        //api/auth/login
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
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return BadRequest("Your account is lock");
        }
    }
}
