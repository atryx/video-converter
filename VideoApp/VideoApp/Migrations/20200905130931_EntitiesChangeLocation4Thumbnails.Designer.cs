﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoApp.Web.Database;

namespace VideoApp.Web.Migrations
{
    [DbContext(typeof(VideoInformationContext))]
    [Migration("20200905130931_EntitiesChangeLocation4Thumbnails")]
    partial class EntitiesChangeLocation4Thumbnails
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VideoApp.Web.Models.Entities.HLSFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FileDirectory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Filename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HLSType")
                        .HasColumnType("int");

                    b.Property<int?>("ParentVideoFileId")
                        .HasColumnType("int");

                    b.Property<int>("ParentVideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentVideoFileId");

                    b.ToTable("HLS");
                });

            modelBuilder.Entity("VideoApp.Web.Models.Entities.Thumbnail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FileDirectory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Format")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentVideoFileId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Timestamp")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("ParentVideoFileId");

                    b.ToTable("Thumbnails");
                });

            modelBuilder.Entity("VideoApp.Web.Models.Entities.VideoFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("BitRate")
                        .HasColumnType("bigint");

                    b.Property<string>("Codec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<string>("FileDirectory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Filename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Framerate")
                        .HasColumnType("float");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<int?>("ParentVideoFileId")
                        .HasColumnType("int");

                    b.Property<string>("PixelFormat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ratio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentVideoFileId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("VideoApp.Web.Models.Entities.HLSFile", b =>
                {
                    b.HasOne("VideoApp.Web.Models.Entities.VideoFile", "ParentVideoFile")
                        .WithMany("HLSFiles")
                        .HasForeignKey("ParentVideoFileId");
                });

            modelBuilder.Entity("VideoApp.Web.Models.Entities.Thumbnail", b =>
                {
                    b.HasOne("VideoApp.Web.Models.Entities.VideoFile", "ParentVideoFile")
                        .WithMany("Thumbnails")
                        .HasForeignKey("ParentVideoFileId");
                });

            modelBuilder.Entity("VideoApp.Web.Models.Entities.VideoFile", b =>
                {
                    b.HasOne("VideoApp.Web.Models.Entities.VideoFile", "ParentVideoFile")
                        .WithMany("AvailableResolutions")
                        .HasForeignKey("ParentVideoFileId");
                });
#pragma warning restore 612, 618
        }
    }
}
