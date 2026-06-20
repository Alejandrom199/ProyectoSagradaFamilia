using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class PlantillaCorreoMappingProfile : Profile
    {
        public PlantillaCorreoMappingProfile()
        {
            CreateMap<PlantillaCorreo, PlantillaCorreoDto.Response>();

            CreateMap<PlantillaCorreoDto.Create, PlantillaCorreo>();

            CreateMap<PlantillaCorreoDto.Update, PlantillaCorreo>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore()); // Codigo nunca se actualiza
        }
    }
}
