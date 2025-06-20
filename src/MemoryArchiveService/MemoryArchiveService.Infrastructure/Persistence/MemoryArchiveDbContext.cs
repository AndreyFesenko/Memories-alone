// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Persistence/MemoryArchiveDbContext.cs
using MemoryArchiveService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MemoryArchiveService.Infrastructure.Persistence;

public class MemoryArchiveDbContext : DbContext
{
    public MemoryArchiveDbContext(DbContextOptions<MemoryArchiveDbContext> options)
        : base(options) { }

    public DbSet<Memory> Memories => Set<Memory>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<MemoryTag> MemoryTags { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }

}