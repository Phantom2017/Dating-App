using System.Linq;
using API.Extensions;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,MemberDto>()
            .ForMember(m=>m.PhotoUrl,opt=>opt.MapFrom(srs=>srs.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDto>();
            CreateMap<MemberUpdateDto,User>();
            CreateMap<UserForRegisterDto,User>();

            CreateMap<Message,MessageDto>()
            .ForMember(dest=>dest.SenderPhotoUrl,opt=>opt.MapFrom(src
            =>src.Sender.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(dest=>dest.RecipientPhotoUrl,opt=>opt.MapFrom(src
            =>src.Recipient.Photos.FirstOrDefault(x=>x.IsMain).Url));
        }
    }
}