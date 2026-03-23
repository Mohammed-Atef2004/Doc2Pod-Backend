using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PodcastConfiguration : IEntityTypeConfiguration<Podcast>
    {
        public void Configure(EntityTypeBuilder<Podcast> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ScriptPath)
                .IsRequired();

            builder.Property(p => p.AudioPath)
                .IsRequired();

            builder.Property(p => p.Mode)
                .IsRequired();

            builder.HasOne(p => p.Document)
                .WithMany(d => d.Podcasts)
                .HasForeignKey(p => p.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
