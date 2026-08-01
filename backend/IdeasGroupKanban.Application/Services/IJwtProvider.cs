using IdeasGroupKanban.Domain.Entities;

namespace IdeasGroupKanban.Application.Services;

public interface IJwtProvider
{
    string Generate(User user);
}
