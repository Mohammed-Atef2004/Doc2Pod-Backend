using Application.Features.Users.Queries.GetUserByEmail;
using Application.Features.Users.Queries.GetUserById;
using Application.Features.Users.Queries.GetUsersByRole;
using AutoMapper;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.Mappings
{
    public sealed class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, GetUserByIdResponse>()
     .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
     .ForCtorParam("Username", opt => opt.MapFrom(src => src.Username.Value))
     .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email.Value))
     .ForCtorParam("Role", opt => opt.MapFrom(src => src.Role.ToString()))
     .ForCtorParam("IsActive", opt => opt.MapFrom(src => src.IsActive))
     .ForCtorParam("IsEmailConfirmed", opt => opt.MapFrom(src => src.IsEmailConfirmed))
     .ForCtorParam("LastLogin", opt => opt.MapFrom(src => src.LastLoginAt));

            CreateMap<User, GetUserByEmailResponse>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLoginAt));

            CreateMap<User, GetUsersByRoleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
