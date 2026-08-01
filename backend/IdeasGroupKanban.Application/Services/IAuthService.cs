using IdeasGroupKanban.Application.DTOs;

namespace IdeasGroupKanban.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
}
