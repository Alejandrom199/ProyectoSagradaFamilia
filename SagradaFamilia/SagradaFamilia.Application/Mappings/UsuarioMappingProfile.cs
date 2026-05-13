using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class UsuarioMappingProfile : Profile
    {
        public UsuarioMappingProfile()
        {

            CreateMap<UsuarioDto.Create, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UsuarioDto.Update, Usuario>();


            CreateMap<Usuario, UsuarioDto.ListResponse>()
                .ForMember(dest => dest.RolNombre,
                    opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty))

                .ForMember(dest => dest.EsMedico,
                    opt => opt.MapFrom(src => src.Medico != null))
                .ForMember(dest => dest.EsPadre,
                    opt => opt.MapFrom(src => src.Padre != null));

            CreateMap<Usuario, UsuarioDto.DetailResponse>()
                .ForMember(dest => dest.RolNombre,
                    opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty))

                .ForMember(dest => dest.MedicoId,
                    opt => opt.MapFrom(src => src.Medico != null ? src.Medico.Id : (int?)null))
                .ForMember(dest => dest.PadreId,
                    opt => opt.MapFrom(src => src.Padre != null ? src.Padre.Id : (int?)null));
        }
    }
}