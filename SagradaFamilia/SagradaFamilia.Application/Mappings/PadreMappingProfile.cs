using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class PadreMappingProfile : Profile
    {
        public PadreMappingProfile()
        {
            CreateMap<PadreDto.Create, Padre>();
            CreateMap<PadreDto.Update, Padre>();

            CreateMap<Padre, PadreDto.ListResponse>()
                .ForMember(dest => dest.NombreCompleto,
                    opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))

                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty))
                .ForMember(dest => dest.Activo,
                    opt => opt.MapFrom(src => src.Usuario != null && src.Usuario.Activo))

                .ForMember(dest => dest.NombreMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))

                .ForMember(dest => dest.TotalHijos,
                    opt => opt.MapFrom(src => src.Ninos != null ? src.Ninos.Count : 0));

            CreateMap<Padre, PadreDto.DetailResponse>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty))
                .ForMember(dest => dest.Activo,
                    opt => opt.MapFrom(src => src.Usuario != null && src.Usuario.Activo))

                .ForMember(dest => dest.MedicoNombreCompleto,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))

                .ForMember(dest => dest.Hijos, opt => opt.MapFrom(src => src.Ninos));
        }
    }
}