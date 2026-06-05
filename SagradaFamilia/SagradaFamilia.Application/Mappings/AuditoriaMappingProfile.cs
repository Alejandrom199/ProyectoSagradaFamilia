using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class AuditoriaMappingProfile : Profile
    {
        public AuditoriaMappingProfile()
        {
            CreateMap<AuditoriaDto.Create, Auditoria>();

            CreateMap<Auditoria, AuditoriaDto.Response>()
                .ForMember(dest => dest.UsuarioEmail,
                    opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty))
                .ForMember(dest => dest.UsuarioNombreCompleto,
                    opt => opt.MapFrom(src =>
                        src.Usuario != null && src.Usuario.Medico != null
                            ? $"{src.Usuario.Medico.Nombre} {src.Usuario.Medico.Apellido}"
                            : (string?)null));
        }
    }
}