using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class CitaMappingProfile : Profile
    {
        public CitaMappingProfile()
        {
            CreateMap<CitaDto.Create, Cita>();
            CreateMap<CitaDto.Update, Cita>();

            CreateMap<Cita, CitaDto.Response>()
                .ForMember(dest => dest.NombreNino,
                    opt => opt.MapFrom(src => src.Nino != null ? $"{src.Nino.Nombre} {src.Nino.Apellido}" : string.Empty))
                .ForMember(dest => dest.NombreMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))
                .ForMember(dest => dest.Estado,
                    opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.TienePrescripcion,
                    opt => opt.MapFrom(src => src.Consulta != null && src.Consulta.Prescripciones.Any()))
                .ForMember(dest => dest.Prescripcion,
                    opt => opt.MapFrom(src => src.Consulta != null ? src.Consulta.Prescripciones.FirstOrDefault() : null))
                .ForMember(dest => dest.Consulta,
                    opt => opt.MapFrom(src => src.Consulta));
        }
    }
}