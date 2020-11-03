﻿using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<UserReponse>> GetAllUserAsync()
        {
            var user = await _userManager.Users.Select(x => new UserReponse()
            {
                
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Dob = x.Dob,
                PhoneNumber = x.PhoneNumber
            }).ToListAsync();
            return user;
        }

        public async Task<UserMessageReponse> LoginUserAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new UserMessageReponse()
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
                        expires: DateTime.UtcNow.AddMinutes(2),
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );
                string TokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new UserMessageReponse()
                {
                    Message = TokenAsString,
                    IsSuccess = true,
                    ExpireDate = token.ValidTo
                };
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };
            }
           

        }

        public async Task<UserMessageReponse> RegisterUserAsync(UserRegisterRequest request)
        {
            if (request == null)
            {
                throw new NullReferenceException("Register Model is null");
            }
            if (request.Password != request.ConfirmPassword)
            {
                return new UserMessageReponse()
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
                return new UserMessageReponse()
                {
                    Message = "User created Successfully!",
                    IsSuccess = true,
                };
            }
            return new UserMessageReponse()
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = resutl.Errors.Select(e => e.Description)
            };

        }
    }
}