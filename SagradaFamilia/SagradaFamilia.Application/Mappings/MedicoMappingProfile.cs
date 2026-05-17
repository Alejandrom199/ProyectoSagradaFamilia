using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class MedicoMappingProfile : Profile
    {
        public MedicoMappingProfile()
        {

            CreateMap<MedicoDto.Create, Medico>();
            CreateMap<MedicoDto.Update, Medico>();


            CreateMap<Medico, MedicoDto.ListResponse>()
                .ForMember(dest => dest.Nombre,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido,
                    opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty));

            CreateMap<Medico, MedicoDto.DetailResponse>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty))

                .ForMember(dest => dest.Pacientes,
                    opt => opt.MapFrom(src => src.Ninos));
        }
    }
}