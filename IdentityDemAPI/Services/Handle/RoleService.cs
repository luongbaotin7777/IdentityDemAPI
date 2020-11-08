using IdentityDemo.API.Dtos;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IdentityDemo.API.Services.Handle
{

    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleService(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<RoleMessageReponse> CreateRole(CreateRoleRequest request)
        {
            if (request == null)
            {
                throw new NullReferenceException("Register Model is null");
            }
            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role != null)
            {
                return new RoleMessageReponse()
                {
                    Message = "Role Name is already existed!",
                    IsSuccess = false,
                };
            }
            role = new AppRole()
            {
                Name = request.Name,
                Description = request.Description
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var userRole = await _roleManager.FindByNameAsync(role.Name);
                if (userRole.Name == "Admin")
                {
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.Create));
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.Edit));
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.Delete));
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.View));
                }
                if (userRole.ToString() == "Mod")
                {
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.Edit));
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.View));
                }
                if (userRole.ToString() == "User")
                {
                    await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, Permission.View));
                }

                return new RoleMessageReponse()
                {
                    Message = "Role created Successfully!",
                    IsSuccess = true,
                };
            }
            return new RoleMessageReponse()
            {
                Message = "Role did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<RoleMessageReponse> DeleteRole(Guid Id)
        {
            var role = await _roleManager.FindByIdAsync(Id.ToString());
            if (role == null) throw new Exception($"{Id} not found");
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return new RoleMessageReponse()
                {
                    Message = "Role Deleted",
                    IsSuccess = true,
                };
            }
            return new RoleMessageReponse()
            {
                Message = " Delete Failed",
                IsSuccess = false
            };
        }


        public async Task<List<RoleReponse>> GetAllRole(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var role = _roleManager.Roles.Where(x => x.Name.Contains(Name));
                var result = await role.Select(r => new RoleReponse()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToListAsync();
                return result;
            }
            else
            {
                var role = await _roleManager.Roles.Select(r => new RoleReponse()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToListAsync();
                return role;
            }
        }

        public async Task<RoleReponse> GetRoleById(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id.ToString());
            if (role == null) throw new Exception($"{Id} not Found");
            var data = new RoleReponse()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description

            };
            return data;

        }

        public async Task<RoleMessageReponse> UpdateRole(Guid Id, UpdateRoleRequest request)
        {
            if (await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != Id))
            {
                return new RoleMessageReponse()
                {
                    Message = "Name Already Exist",
                    IsSuccess = true
                };
            }
            var role = await _roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
            {
                return new RoleMessageReponse()
                {
                    Message = "Id not found",
                    IsSuccess = false
                };
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                role.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                role.Description = request.Description;
            }

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return new RoleMessageReponse()
                {
                    Message = "Role Updated"
                };
            }
            return new RoleMessageReponse()
            {
                Message = "Update Failed",
                IsSuccess = true
            };

        }
    }
}
