using AutoMapper;
using SagradaFamilia.Application.DTOs.Medidas;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class MedidaMappingProfile : Profile
    {
        public MedidaMappingProfile()
        {
            CreateMap<CrearMedidaRequest, Medida>();
            CreateMap<ActualizarMedidaRequest, Medida>();

            CreateMap<Medida, MedidaResponse>()
                .ForMember(dest => dest.NombreNino,
                    opt => opt.MapFrom(src => src.Nino != null
                        ? $"{src.Nino.Nombre} {src.Nino.Apellido}"
                        : string.Empty))
                .ForMember(dest => dest.EstadoNutricional, opt => opt.Ignore())
                .ForMember(dest => dest.Percentil, opt => opt.Ignore());
        }
    }
}
