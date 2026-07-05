using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class ConsultaMappingProfile : Profile
    {
        public ConsultaMappingProfile()
        {
            CreateMap<ConsultaDto.Actualizar, Consulta>();

            CreateMap<Consulta, ConsultaDto.Response>()
                .ForMember(dest => dest.Estado,
                    opt => opt.MapFrom(src => src.Estado.ToString()));
        }
    }
}
