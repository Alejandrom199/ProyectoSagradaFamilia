using AutoMapper;
using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<Usuario, UsuarioResponse>()
                .ForMember(dest => dest.RolNombre,
                    opt => opt.MapFrom(src => src.Rol != null
                        ? src.Rol.Nombre
                        : string.Empty))
                .ForMember(dest => dest.TotalHijos, opt => opt.MapFrom(src => src.Ninos.Count));
        }
    }
}
