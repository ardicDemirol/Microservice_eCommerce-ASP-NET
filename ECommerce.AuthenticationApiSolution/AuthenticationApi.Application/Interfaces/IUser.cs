using AuthenticationApi.Application.DTOs;
using ECommerce.SharedLibrary.Response;

namespace AuthenticationApi.Application.Interfaces;
public interface IUser
{
    Task<Response> Register(AppUserDto appUserDto);
    Task<Response> Login(LoginDto loginDto);
    Task<GetUserDto> GetUser(int userId);

}
