﻿using IdentityDemo.API.BaseRepository;
using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IPasswordHasher<AppUser> _passwordHasher;
       
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IConfiguration configuration, IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
           
        }

        public async Task<UserMessageReponse> DeleteUserAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null) throw new Exception($"{Id} not Found");
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new UserMessageReponse()
                {
                    Message = "User Deleted",
                    IsSuccess = true
                };
            }
            return new UserMessageReponse()
            {
                Message = "Delete Failed",
                IsSuccess = false
            };

        }

        public async Task<List<UserReponse>> GetAllUserAsync(string UserName,string Email)
        {
            
            if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Email))
            {
                var user = _userManager.Users.Where(x => x.UserName.Contains(UserName) || x.Email.Contains(Email));
                var result = await user.Select(x => new UserReponse()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Dob = x.Dob,
                    PhoneNumber = x.PhoneNumber
                }).ToListAsync();
                return result;
            }
            else
            {
                var user = await _userManager.Users.Select(x => new UserReponse()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Dob = x.Dob,
                    PhoneNumber = x.PhoneNumber
                }).ToListAsync();
                return user;
            }
            
            
        }

        public async Task<UserReponse> GetUserByIdAsync(string Id)
        {
            var users = await _userManager.FindByIdAsync(Id.ToString());
            if (users == null) throw new Exception($"{Id} not found");
            var data = new UserReponse()
            {
                Id = users.Id,
                UserName = users.UserName,
                FirstName = users.FirstName,
                LastName = users.LastName,
                Dob = users.Dob,
                Email = users.Email,
                PhoneNumber = users.PhoneNumber
            };
            return data;

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
                        expires: DateTime.UtcNow.AddDays(7),
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
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new UserMessageReponse()
                {
                    Message = "Username is already existed!",
                    IsSuccess = false,
                };
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new UserMessageReponse()
                {
                    Message = "Email is already existed!",
                    IsSuccess = false,
                };
            }
            user = new AppUser()
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

        public async Task<UserMessageReponse> UpdateUserAsync(Guid Id, UserUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != Id))
            {
                return new UserMessageReponse()
                {
                    Message = "Email is Already Existed",
                    IsSuccess = false
                };
            }
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
            {
                return new UserMessageReponse()
                {
                    Message = "Id not Found",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "First name cannot be empty",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.LastName))
            {
                user.LastName = request.LastName;
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "Last name cannot be empty",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "PhoneNumber cannot be empty",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "Email cannot be empty",
                    IsSuccess = false
                };
            }
            if (request.Dob.HasValue)
            {
                user.Dob = (DateTime)request.Dob;
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "Dob cannot be empty",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            }
            else
            {
                return new UserMessageReponse()
                {
                    Message = "PassWord cannot be empty",
                    IsSuccess = false
                };
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return new UserMessageReponse()
                {
                    Message = "User Updated",
                    IsSuccess = true

                };
            return new UserMessageReponse()
            {
                Message = "Update Failed",
                IsSuccess = true
            };
        }
    }
}
