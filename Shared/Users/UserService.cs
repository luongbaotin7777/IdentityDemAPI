using IdentityDemoAPI.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<UserReponse> LoginUserAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new UserReponse()
                {
                    Message = "There is no user with  that UserName",
                    IsSuccess = false
                };
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.Remeberme, false);
            if (result.Succeeded)
            
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claim = new[]
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.GivenName,user.FirstName),
                    new Claim(ClaimTypes.Surname,user.LastName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,string.Join(";",roles))
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
                var token = new JwtSecurityToken(
                        issuer: _configuration["AuthSettings:Issuer"],
                        audience: _configuration["AuthSettings:Audience"],
                        claims: claim,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );
                string TokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new UserReponse()
                {
                    Message = TokenAsString,
                    IsSuccess = true,
                    ExpireDate = token.ValidTo
                };
            }
            else
            {
                return new UserReponse()
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };
            }
           

        }

        public async Task<UserReponse> RegisterUserAsync(UserRegisterRequest request)
        {
            if (request == null)
            {
                throw new NullReferenceException("Register Model is null");
            }
            if (request.Password != request.ConfirmPassword)
            {
                return new UserReponse()
                {
                    Message = "Confirm password doestn't match the password",
                    IsSuccess = false,
                };
            };
            var user = new AppUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };

            var resutl = await _userManager.CreateAsync(user, request.Password);
            if (resutl.Succeeded)
            {
                return new UserReponse()
                {
                    Message = "User created Successfully!",
                    IsSuccess = true,
                };
            }
            return new UserReponse()
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = resutl.Errors.Select(e => e.Description)
            };

        }
    }
}
