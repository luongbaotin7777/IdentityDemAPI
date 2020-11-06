using IdentityDemo.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Services.Interface
{

   public interface IRoleService
    {
        Task<RoleMessageReponse> CreateRole(CreateRoleRequest request);
        Task<List<RoleReponse>> GetAllRole(string Name);
        Task<RoleReponse> GetRoleById(string Id);
        Task<RoleMessageReponse> UpdateRole(Guid Id,UpdateRoleRequest request);
        Task<RoleMessageReponse> DeleteRole(Guid Id);
        //Task<RoleMessageReponse> AddUserToRole(string UserName, string RoleName);

    }
}
