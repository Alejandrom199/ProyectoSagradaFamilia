using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // 1. Mapeo para Admin
            CreateMap<Usuario, UsuarioDto.ListResponse>()
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));

            // 2. Mapeo para Padre (Toma datos del Padre y de la entidad Usuario ligada)
            CreateMap<Padre, PadreDto.ListResponse>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Usuario.Activo))
                .ForMember(dest => dest.TotalHijos, opt => opt.MapFrom(src => src.Ninos.Count));

            CreateMap<Padre, PadreDto.DetailResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Usuario.Activo))
                .ForMember(dest => dest.Hijos, opt => opt.MapFrom(src => src.Ninos));

            // 3. Mapeo para Medico
            CreateMap<Medico, MedicoDto.ListResponse>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));

            CreateMap<Medico, MedicoDto.DetailResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));
        }
    }
}