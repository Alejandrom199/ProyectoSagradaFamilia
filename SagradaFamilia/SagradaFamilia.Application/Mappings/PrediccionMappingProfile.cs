using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class PrediccionMappingProfile : Profile
    {
        public PrediccionMappingProfile()
        {
            CreateMap<Prediccion, PrediccionDto.Punto>();
        }
    }
}