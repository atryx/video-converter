using Microsoft.EntityFrameworkCore;
using System;
using VideoApp.Web.Models.Entities;

namespace VideoApp.Web.Database
{
    public class VideoInformationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=VideoConverterDatabase;Trusted_Connection=True;");
        }

        public DbSet<VideoFile> Videos { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }
        public DbSet<HLSFile> HLS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoFile>()
                .Property(e => e.Status)
                 .HasConversion(
                        v => v.ToString(),
                        v => (Status)Enum.Parse(typeof(Status), v));
            modelBuilder.Entity<VideoFile>().ToTable("Videos")
                    .HasMany(v => v.AvailableResolutions)
                    .WithOne(p => p.ParentVideoFile)
                    .HasForeignKey(c => c.ParentVideoFileId);
            modelBuilder.Entity<Thumbnail>().ToTable("Thumbnails");
            modelBuilder.Entity<HLSFile>().ToTable("HLS");
        }
    }
}
