using Microsoft.EntityFrameworkCore;
using System;
using VideoApp.Web.Models;
using VideoApp.Web.Models.Entities;

namespace VideoApp.Web.Database
{
    public class VideoInformationContext : DbContext
    {
        public VideoInformationContext(DbContextOptions<VideoInformationContext> options) : base(options)
        {

        }



        public DbSet<AvailableResolution> AvailableResolutions { get; set; }
        public DbSet<VideoFile> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvailableResolution>().ToTable("AvailableResolutions");
            modelBuilder.Entity<VideoFile>()
                .Property(e => e.Status)
                 .HasConversion(
                        v => v.ToString(),
                        v => (Status)Enum.Parse(typeof(Status), v));
            modelBuilder.Entity<VideoFile>().ToTable("Videos")
                    .HasMany(v => v.DifferentResolutionsFile)
                    .WithOne(p => p.ParentVideoFile)
                    .HasForeignKey(c => c.ParentVideoFileId);
            //modelBuilder.Entity<Student>().ToTable("Student");
        }

        //protected override void Seed(VideoInformationContext context)
        //{
        //    context.AvailableResolutions.Add(x => x.Id,
        //        new AvailableResolution()
        //        {
        //            Id = 1,
        //            Name = "480p",
        //            Resolution = new Resolution()
        //            {
        //                Width = 720,
        //                Height = 480
        //            },
        //            Category = Category.EDTV
        //        },
        //        new AvailableResolution()
        //        {
        //            Id = 2,
        //            Name = "576p",
        //            Resolution = new Resolution()
        //            {
        //                Width = 720,
        //                Height = 576
        //            },
        //            Category = Category.EDTV
        //        },
        //         new AvailableResolution()
        //         {
        //             Id = 3,
        //             Name = "720p",
        //             Resolution = new Resolution()
        //             {
        //                 Width = 1280,
        //                 Height = 720
        //             },
        //             Category = Category.HDTV
        //         },
        //          new AvailableResolution()
        //          {
        //              Id = 4,
        //              Name = "1080p",
        //              Resolution = new Resolution()
        //              {
        //                  Width = 1980,
        //                  Height = 1080
        //              },
        //              Category = Category.HDTV
        //          },
        //           new AvailableResolution()
        //           {
        //               Id = 5,
        //               Name = "4K UHD",
        //               Resolution = new Resolution()
        //               {
        //                   Width = 3840,
        //                   Height = 2160
        //               },
        //               Category = Category.UHDTV
        //           },
        //            new AvailableResolution()
        //            {
        //                Id = 6,
        //                Name = "DCI 4K",
        //                Resolution = new Resolution()
        //                {
        //                    Width = 4096,
        //                    Height = 2160
        //                },
        //                Category = Category.UHDTV
        //            },
        //             new AvailableResolution()
        //             {
        //                 Id = 7,
        //                 Name = "8K UHD",
        //                 Resolution = new Resolution()
        //                 {
        //                     Width = 7680,
        //                     Height = 4320
        //                 },
        //                 Category = Category.UHDTV
        //             }
        //        );
        //}
    }
}
