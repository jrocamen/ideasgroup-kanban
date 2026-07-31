using IdeasGroupKanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasGroupKanban.Infrastructure.Data.Configurations;

public class KanbanTaskConfiguration : IEntityTypeConfiguration<KanbanTask>
{
    public void Configure(EntityTypeBuilder<KanbanTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(50);
        
        builder.HasOne(t => t.Assignee)
               .WithMany(u => u.Tasks)
               .HasForeignKey(t => t.AssigneeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
