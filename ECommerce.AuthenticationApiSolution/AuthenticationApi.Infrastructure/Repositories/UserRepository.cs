using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using ECommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories;

public class UserRepository(AuthenticationDbContext dbContext, IConfiguration config) : IUser
{
    private async Task<AppUser> GetUserByEmail(string email)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
        return user!;
    }

    public async Task<GetUserDto> GetUser(int userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        return user is not null ? new GetUserDto(
            user.Id,
            user.Name,
            user.TelephoneNumber,
            user.Address,
            user.Email,
            user.Role) : null!;
    }

    public async Task<Response> Login(LoginDto loginDto)
    {
        var getUser = await GetUserByEmail(loginDto.Email!);
        if (getUser is null) return new Response(false, "Invalid login details");


        //bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
        bool verifyPassword = loginDto.Password.Equals(getUser.Password);
        if (!verifyPassword) return new Response(false, "Invalid login details");

        string token = GenerateToken(getUser);
        return new Response(true, token);
    }

    public async Task<Response> Register(AppUserDto appUserDto)
    {
        var getUser = await GetUserByEmail(appUserDto.Email!);
        if (getUser is not null) return new Response(false, "You can't use this email for registration");

        var result = dbContext.Users.Add(new AppUser
        {
            Name = appUserDto.Name,
            TelephoneNumber = appUserDto.TelephoneNumber,
            Address = appUserDto.Address,
            Email = appUserDto.Email,
            Password = appUserDto.Password,
            Role = appUserDto.Role
        });

        await dbContext.SaveChangesAsync();
        return result.Entity.Id > 0 ? new Response(true, "User registered successfully") : new Response(false, "Failed to register user");
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.ASCII.GetBytes(config.GetSection("Authentication:Key").Value!);
        var securityKey = new SymmetricSecurityKey(key);
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new (ClaimTypes.Name, user.Name!),
            new (ClaimTypes.Email, user.Email!),
        ];

        if (string.IsNullOrEmpty(user.Role)) claims.Add(new Claim(ClaimTypes.Role, user.Role!));

        var token = new JwtSecurityToken(
            issuer: config["Authentication:Issuer"],
            audience: config["Authentication:Audience"],
            claims: claims,
            expires: null,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
