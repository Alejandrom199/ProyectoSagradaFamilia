using AutoMapper;
using SagradaFamilia.Application.DTOs.Predicciones;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class PrediccionMappingProfile : Profile
    {
        public PrediccionMappingProfile()
        {
            CreateMap<Prediccion, PuntoPrediccion>();
        }
    }
}
