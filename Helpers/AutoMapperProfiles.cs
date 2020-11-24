using System.Linq;
using api.Dtos;
using api.Models;
using AutoMapper;

namespace api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForDetailDto>().ForMember(
                dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(
                        src => src.Photos.FirstOrDefault(p => p.isMain).Url
                    );
                }
            ).ForMember(
                dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DOB.CalcAge());
                }
            );

            CreateMap<User, UserForListDto>().ForMember(
                dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(
                        src => src.Photos.FirstOrDefault(p => p.isMain).Url
                    );
                }
            ).ForMember(
                dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DOB.CalcAge());
                }
            );

            CreateMap<Photo, PhotoForDetailDto>();

            CreateMap<UserForUpdateDto, User>();
        }
    }
}