using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class MedidaMappingProfile : Profile
    {
        public MedidaMappingProfile()
        {

            CreateMap<MedidaDto.Create, Medida>();
            CreateMap<MedidaDto.Update, Medida>();


            CreateMap<Medida, MedidaDto.Response>()
                .ForMember(dest => dest.NombreNino,
                    opt => opt.MapFrom(src => src.Nino != null ? $"{src.Nino.Nombre} {src.Nino.Apellido}" : string.Empty))

                .ForMember(dest => dest.NombreMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))

                .ForMember(dest => dest.EstadoNutricional, opt => opt.Ignore())
                .ForMember(dest => dest.PercentilPeso, opt => opt.Ignore())
                .ForMember(dest => dest.PercentilTalla, opt => opt.Ignore());
        }
    }
}