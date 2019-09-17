using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt =>
            {
                opt.MapFrom(r => r.DateOfBirth.CalculateAge());
            })
            ;
            CreateMap<User, UserForDetailedDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt =>
            {
                opt.MapFrom(r => r.DateOfBirth.CalculateAge());
            })
            ;

            CreateMap<Photo, PhotosToDetailedDto>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForUpdatesDto, User>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<Message, MessageDto>();
            CreateMap<MessageForCreationDto, Message>();
            CreateMap<Message, MessageToReturnDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt =>
            {
                opt.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url);
            })
            .ForMember(dest => dest.RecipientPhotoUrl, opt =>
            {
                opt.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url);
            });
        }
    }
}