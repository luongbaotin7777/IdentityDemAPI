using IdentityDemo.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Services.Interface
{
    public interface IUserService
    {
        Task<UserReponse> GetUserByIdAsync(string Id);
        Task<UserMessageReponse> UpdateUserAsync(Guid Id,UserUpdateRequest request);
        Task<List<UserReponse>> GetAllUserAsync(string UserName,string Email);
        Task<UserMessageReponse> RegisterUserAsync(UserRegisterRequest request);
        Task<UserMessageReponse> DeleteUserAsync(string Id);
        Task<UserMessageReponse> LoginUserAsync(UserLoginRequest request);
        //Task<UserMessageReponse> RegisterAdminAsync(UserRegisterRequest request);
        //Task<UserMessageReponse> RoleAssign(Guid Id, RoleAssignRequest request); 


    }
}
