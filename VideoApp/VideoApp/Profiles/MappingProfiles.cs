using AutoMapper;
using System.Linq;
using VideoApp.Web.Models.Entities;
using VideoApp.Web.Models.ViewModels;
using Xabe.FFmpeg;

namespace VideoApp.Web.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<VideoFile, VideoFileModel>()
                .ForMember(dest => dest.Resolution, opt => opt.MapFrom(src => $"{src.Width}:{src.Height}"))
                .ForMember(dest => dest.CodecName, opt => opt.MapFrom(src => src.Codec))
                .ReverseMap();
            CreateMap<Thumbnail, ThumbnailModel>();
            CreateMap<HLSFile, HLSFileModel>();
            CreateMap<IMediaInfo, VideoFile>()
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Width))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Height))
                .ForMember(dest => dest.Codec, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Codec))
                .ForMember(dest => dest.Framerate, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Framerate))
                .ForMember(dest => dest.Ratio, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Ratio))
                .ForMember(dest => dest.BitRate, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().Bitrate))
                .ForMember(dest => dest.PixelFormat, opt => opt.MapFrom(src => src.VideoStreams.FirstOrDefault().PixelFormat));
        }
        
    }
}
