using Authentication.Models;
using Authentication.Models.DTOs;

namespace Authentication.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<LoginDto>> Authentication(string username, string password);

        Task<ResponseDto> CreateUser(UserRegistrationModel model);
    }
}
