// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Persistence/MemoryArchiveDbContext.cs

using Microsoft.EntityFrameworkCore;
using MemoryArchiveService.Domain.Entities;

namespace MemoryArchiveService.Infrastructure.Persistence;

public class MemoryArchiveDbContext : DbContext
{
    public MemoryArchiveDbContext(DbContextOptions<MemoryArchiveDbContext> options)
        : base(options) { }

    public DbSet<Memory> Memories => Set<Memory>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<MemoryTag> MemoryTags => Set<MemoryTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Устанавливаем схему по умолчанию
        modelBuilder.HasDefaultSchema("memory");

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FileName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Url)
                  .IsRequired()
                  .HasMaxLength(1000);

            entity.Property(e => e.StorageUrl)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity.Property(e => e.OwnerId)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.Property(e => e.MediaType)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Memory)
                  .WithMany(m => m.MediaFiles)
                  .HasForeignKey(e => e.MemoryId);
        });

        modelBuilder.Entity<MemoryTag>(entity =>
        {
            entity.HasKey(mt => new { mt.MemoryId, mt.TagId });

            entity.HasOne(mt => mt.Memory)
                  .WithMany(m => m.MemoryTags)
                  .HasForeignKey(mt => mt.MemoryId);

            entity.HasOne(mt => mt.Tag)
                  .WithMany(t => t.MemoryTags)
                  .HasForeignKey(mt => mt.TagId);
        });

        // Подключение внешних конфигураций при необходимости
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MemoryArchiveDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
