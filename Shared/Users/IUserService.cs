using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Users
{
    public interface IUserService
    {
        Task<UserReponse> RegisterUserAsync(UserRegisterRequest request);
        Task<UserReponse> LoginUserAsync(UserLoginRequest request);
    }
}
