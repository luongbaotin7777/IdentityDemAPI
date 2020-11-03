using IdentityDemo.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.API.Services.Interface
{
    public interface IUserService
    {
        Task<List<UserReponse>> GetAllUserAsync();
        Task<UserMessageReponse> RegisterUserAsync(UserRegisterRequest request);
        Task<UserMessageReponse> LoginUserAsync(UserLoginRequest request);
    }
}
