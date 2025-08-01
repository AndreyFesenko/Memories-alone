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
        modelBuilder.HasDefaultSchema("memory");

        // Memory
        modelBuilder.Entity<Memory>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Description).HasMaxLength(2000);

            // Memory ↔ Tag (через MemoryTag)
            entity.HasMany(m => m.MemoryTags)
                  .WithOne(mt => mt.Memory)
                  .HasForeignKey(mt => mt.MemoryId);
        });

        // MediaFile
        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(mf => mf.Id);
            entity.Property(mf => mf.FileName).IsRequired().HasMaxLength(255);
            entity.Property(mf => mf.Url).IsRequired().HasMaxLength(1000);
            entity.Property(mf => mf.StorageUrl).IsRequired().HasMaxLength(2000);
            entity.Property(mf => mf.OwnerId).IsRequired().HasMaxLength(64);

            entity.HasOne(mf => mf.Memory)
                  .WithMany(m => m.MediaFiles)
                  .HasForeignKey(mf => mf.MemoryId);
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);

            entity.HasMany(t => t.MemoryTags)
                  .WithOne(mt => mt.Tag)
                  .HasForeignKey(mt => mt.TagId);
        });

        // MemoryTag (связующая таблица)
        modelBuilder.Entity<MemoryTag>(entity =>
        {
            entity.ToTable("MemoryTags", "memory");
            entity.HasKey(mt => new { mt.MemoryId, mt.TagId });
        });

        base.OnModelCreating(modelBuilder);
    }
}
