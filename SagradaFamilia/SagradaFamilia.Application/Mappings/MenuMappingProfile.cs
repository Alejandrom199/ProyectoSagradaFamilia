using AutoMapper;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class MenuMappingProfile : Profile
    {
        public MenuMappingProfile()
        {
            CreateMap<Modulo, MenuResponse>();

            CreateMap<Opcion, OpcionResponse>()
                .ForMember(dest => dest.Acciones,
                    opt => opt.MapFrom(src =>
                        src.OpcionAcciones
                            .Where(oa => oa.Accion != null)
                            .Select(oa => oa.Accion.Nombre)
                            .ToList()));
        }
    }
}
