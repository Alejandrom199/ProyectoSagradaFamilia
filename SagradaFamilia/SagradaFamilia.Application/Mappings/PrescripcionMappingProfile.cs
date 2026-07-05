using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class PrescripcionMappingProfile : Profile
    {
        public PrescripcionMappingProfile()
        {
            CreateMap<PrescripcionDto.Create, Prescripcion>()
                .ForMember(dest => dest.Medicamentos, opt => opt.MapFrom(src => src.Medicamentos));

            CreateMap<PrescripcionDto.Update, Prescripcion>()
                .ForMember(dest => dest.Medicamentos, opt => opt.MapFrom(src => src.Medicamentos));

            CreateMap<Prescripcion, PrescripcionDto.Response>()
                .ForMember(dest => dest.CitaId,
                    opt => opt.MapFrom(src => src.Consulta.CitaId))

                .ForMember(dest => dest.NombreNino,
                    opt => opt.MapFrom(src => src.Nino != null ? $"{src.Nino.Nombre} {src.Nino.Apellido}" : string.Empty))

                .ForMember(dest => dest.NombreMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))

                .ForMember(dest => dest.EspecialidadMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? src.Medico.Especialidad : string.Empty));

            CreateMap<Prescripcion, PrescripcionDto.PrescripcionResumen>();
        }
    }
}
