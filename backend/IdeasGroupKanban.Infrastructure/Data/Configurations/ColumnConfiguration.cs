using IdeasGroupKanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasGroupKanban.Infrastructure.Data.Configurations;

public class ColumnConfiguration : IEntityTypeConfiguration<Column>
{
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        
        builder.HasMany(c => c.Tasks)
               .WithOne(t => t.Column)
               .HasForeignKey(t => t.ColumnId)
               .OnDelete(DeleteBehavior.Restrict); // Important: Restrict deleting a column if it has tasks
    }
}
