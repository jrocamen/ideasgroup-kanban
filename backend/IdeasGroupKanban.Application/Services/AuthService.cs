using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Domain.Interfaces;
using BCrypt.Net;

namespace IdeasGroupKanban.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return null; // User not found
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return null; // Invalid password
        }

        var token = _jwtProvider.Generate(user);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        });
    }
}
