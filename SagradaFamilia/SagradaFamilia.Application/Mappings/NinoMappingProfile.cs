using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class NinoMappingProfile : Profile
    {
        public NinoMappingProfile()
        {

            CreateMap<NinoDto.Create, Nino>();
            CreateMap<NinoDto.Update, Nino>();


            CreateMap<Nino, NinoDto.ListResponse>()
                .ForMember(dest => dest.Nombre,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido,
                    opt => opt.MapFrom(src => src.Apellido))

                .ForMember(dest => dest.NombrePadre,
                    opt => opt.MapFrom(src => src.Padre != null ? $"{src.Padre.Nombre} {src.Padre.Apellido}" : string.Empty))

                .ForMember(dest => dest.NombreMedico,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))

                .ForMember(dest => dest.EdadMeses,
                    opt => opt.MapFrom(src => CalcularEdadMeses(src.FechaNacimiento)));

            CreateMap<Nino, NinoDto.DetailResponse>()
                .ForMember(dest => dest.EdadMeses,
                    opt => opt.MapFrom(src => CalcularEdadMeses(src.FechaNacimiento)))

                .ForMember(dest => dest.PadreNombreCompleto,
                    opt => opt.MapFrom(src => src.Padre != null ? $"{src.Padre.Nombre} {src.Padre.Apellido}" : string.Empty))
                .ForMember(dest => dest.PadreTelefono,
                    opt => opt.MapFrom(src => src.Padre != null ? src.Padre.Telefono : string.Empty))
                .ForMember(dest => dest.PadreEmail,
                    opt => opt.MapFrom(src => src.Padre != null && src.Padre.Usuario != null ? src.Padre.Usuario.Email : string.Empty))

                .ForMember(dest => dest.MedicoNombreCompleto,
                    opt => opt.MapFrom(src => src.Medico != null ? $"{src.Medico.Nombre} {src.Medico.Apellido}" : string.Empty))
                .ForMember(dest => dest.MedicoEspecialidad,
                    opt => opt.MapFrom(src => src.Medico != null ? src.Medico.Especialidad : string.Empty));
        }

        private static int CalcularEdadMeses(DateOnly fechaNacimiento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var meses = ((hoy.Year - fechaNacimiento.Year) * 12) + hoy.Month - fechaNacimiento.Month;
            if (hoy.Day < fechaNacimiento.Day) meses--;
            return Math.Max(0, meses);
        }
    }
}