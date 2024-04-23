using AutoMapper;
using MagicVilla_Api.Modelos;
using MagicVilla_Api.Modelos.Dto;
using System.Runtime;

namespace MagicVilla_Api
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();

            CreateMap<Villa, VillaCreateDto>().ReverseMap();

            CreateMap<Villa, VillaUpdateDto>().ReverseMap();

        }
    }
}
