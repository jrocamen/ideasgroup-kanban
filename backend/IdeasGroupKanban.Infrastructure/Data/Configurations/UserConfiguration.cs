using IdeasGroupKanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasGroupKanban.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();

        // Seed Users
        // In a real app we'd inject IPasswordHasher, here we'll assume the hash is pre-generated (e.g. SHA256 or bcrypt)
        string defaultHash = "$2a$11$i/pzsIcwIc6lwhEApYEgcu/iqq7ikESZhsLWI1d1Kn2dUBlKh7sBy";

        builder.HasData(
            new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice Evaluator", Email = "alice@ideasgroup.test", PasswordHash = defaultHash },
            new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob Developer", Email = "bob@ideasgroup.test", PasswordHash = defaultHash }
        );
    }
}
