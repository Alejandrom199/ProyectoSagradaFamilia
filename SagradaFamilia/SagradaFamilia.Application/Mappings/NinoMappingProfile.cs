using AutoMapper;
using SagradaFamilia.Application.DTOs.Ninos;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class NinoMappingProfile : Profile
    {
        public NinoMappingProfile()
        {
            CreateMap<CrearNinoRequest, Nino>();

            CreateMap<Nino, NinoResponse>()
                .ForMember(dest => dest.NombrePadre,
                    opt => opt.MapFrom(src => src.Representante != null
                        ? $"{src.Representante.Nombre} {src.Representante.Apellido}"
                        : string.Empty))
                .ForMember(dest => dest.EdadMeses,
                    opt => opt.MapFrom(src => CalcularEdadMeses(src.FechaNacimiento)));
        }

        private static int CalcularEdadMeses(DateOnly fechaNacimiento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var meses = ((hoy.Year - fechaNacimiento.Year) * 12)
                      + hoy.Month - fechaNacimiento.Month;

            if (hoy.Day < fechaNacimiento.Day)
                meses--;

            return Math.Max(0, meses);
        }
    }
}
