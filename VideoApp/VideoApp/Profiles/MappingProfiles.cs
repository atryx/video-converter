using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoApp.FFmpegUtilities.Models;
using VideoApp.Web.Models;
using VideoApp.Web.Models.Entities;
using VideoApp.Web.Models.ViewModels;
using Xabe.FFmpeg;

namespace VideoApp.Web.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<VideoInformation, VideoFile>()
                    .ForMember(dest => dest.Filename, opt => opt.MapFrom(src => src.Format.Filename))
                    .ForMember(dest => dest.FormatName, opt => opt.MapFrom(src => src.Format.FormatName))
                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Format.StartTime))
                    .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Format.Size))
                    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Format.Duration))
                    .ForMember(dest => dest.BitRate, opt => opt.MapFrom(src => src.Format.BitRate))
                    .ForMember(dest => dest.CodecName, opt => opt.MapFrom(src =>
                                src.Streams.Where(x => x.CodecType.Equals("video")).FirstOrDefault().CodecType))
                    .ForMember(dest => dest.Height, opt => opt.MapFrom(src =>
                                src.Streams.Where(x => x.CodecType.Equals("video")).FirstOrDefault().Height))
                    .ForMember(dest => dest.Width, opt => opt.MapFrom(src =>
                                src.Streams.Where(x => x.CodecType.Equals("video")).FirstOrDefault().Width))
                    .ForMember(dest => dest.SampleAspectRatio, opt => opt.MapFrom(src =>
                                src.Streams.Where(x => x.CodecType.Equals("video")).FirstOrDefault().SampleAspectRatio))
                    .ForMember(dest => dest.DisplayAspectRatio, opt => opt.MapFrom(src =>
                                src.Streams.Where(x => x.CodecType.Equals("video")).FirstOrDefault().DisplayAspectRatio));
            CreateMap<VideoFile, VideoFileModel>()
                .ForMember(dest => dest.Resolution, opt => opt.MapFrom(src => $"{src.Width}:{src.Height}"))
                .ReverseMap();
            CreateMap<OutputFormat,VideoSize>();
        }
        
    }
}
