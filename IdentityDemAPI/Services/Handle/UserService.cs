using IdentityDemo.API.BaseRepository;
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

        public async Task<List<UserReponse>> GetAllUserAsync(string UserName, string Email)
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
                    PhoneNumber = x.PhoneNumber,


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
                    PhoneNumber = x.PhoneNumber,


                }).ToListAsync();
                return user;
            }


        }

        public async Task<UserReponse> GetUserByIdAsync(string Id)
        {
            var users = await _userManager.FindByIdAsync(Id.ToString());
            if (users == null) throw new Exception($"{Id} not found");
            var roles = await _userManager.GetRolesAsync(users);
            var data = new UserReponse()
            {
                Id = users.Id,
                UserName = users.UserName,
                FirstName = users.FirstName,
                LastName = users.LastName,
                Dob = users.Dob,
                Email = users.Email,
                PhoneNumber = users.PhoneNumber,
                Roles = roles

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

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.Remeberme, true);
            if (result.Succeeded)

            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.GivenName,user.FirstName),
                    new Claim(ClaimTypes.Surname,user.LastName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                    //new Claim(ClaimTypes.Role,string.Join(";",userRoles))
                };
                foreach (var userrole in userRoles)
                {
                    claim.Add(new Claim(ClaimTypes.Role, userrole));
                }
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
                    Message = "Username already exist!",
                    IsSuccess = false,
                };
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new UserMessageReponse()
                {
                    Message = "Email already exist!",
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

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(request.RoleId))
                {
                    var role = await _roleManager.FindByIdAsync(request.RoleId);
                    if (role != null)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                        if (addRoleResult.Succeeded)
                        {
                            return new UserMessageReponse()
                            {
                                Message = "User created Successfully!",
                                IsSuccess = true,
                            };
                        }
                    }
                    return new UserMessageReponse()
                    {
                        Message = "Role Not Found",
                        IsSuccess = false,
                    };

                }

            }
            return new UserMessageReponse()
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        //public async Task<UserMessageReponse> RoleAssign(Guid Id, RoleAssignRequest request)
        //{
        //    var user = await _userManager.FindByIdAsync(Id.ToString());
        //    if (user == null)
        //    {
        //        return new UserMessageReponse()
        //        {
        //            Message = "User Id Not Found",
        //            IsSuccess = false
        //        };
        //    }
        //    var removeRoles = request.SelectedRoles.Where(x => x.Selected == false).Select(x => x.Name).ToList();
        //    await _userManager.RemoveFromRolesAsync(user, removeRoles);
        //    var addRoles = request.SelectedRoles.Where(x => x.Selected).Select(x => x.Name).ToList();
        //    foreach (var roleName in addRoles)
        //    {
        //        if (await _userManager.IsInRoleAsync(user, roleName) == false)
        //        {
        //            await _userManager.AddToRolesAsync(user, addRoles);
        //        }
        //    }
        //    return new UserMessageReponse()
        //    {
        //        Message = "Grant Role Successed",
        //        IsSuccess = true
        //    };

        //}

        public async Task<UserMessageReponse> UpdateUserAsync(Guid Id, UserUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != Id))
            {
                return new UserMessageReponse()
                {
                    Message = "Email Already Exist",
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
            {
                if (!string.IsNullOrEmpty(request.RoleId))
                {
                    var role = await _roleManager.FindByIdAsync(request.RoleId);
                    if (role != null)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                        if (addRoleResult.Succeeded)
                        {
                            return new UserMessageReponse()
                            {
                                Message = "User created Successfully with role!",
                                IsSuccess = true,
                            };
                        }
                    }
                    return new UserMessageReponse()
                    {
                        Message = "Role Not Found",
                        IsSuccess = false,
                    };

                }
                return new UserMessageReponse()
                {
                    Message = "Update Successed",
                    IsSuccess = false
                };

            }
            return new UserMessageReponse()
            {
                Message = "Update Failed",
                IsSuccess = false
            };

        }
    }
}
