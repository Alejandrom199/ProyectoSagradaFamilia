using AutoMapper;
using SagradaFamilia.Application.DTOs.Alimentos;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class AlimentoMappingProfile : Profile
    {
        public AlimentoMappingProfile()
        {
            CreateMap<CrearAlimentoRequest, Alimento>();

            CreateMap<Alimento, AlimentoResponse>()
                .ForMember(dest => dest.CategoriaNombre,
                    opt => opt.MapFrom(src => src.Categoria != null
                        ? src.Categoria.Nombre
                        : string.Empty));

            CreateMap<CrearCategoriaRequest, CategoriaAlimento>();

            CreateMap<CategoriaAlimento, CategoriaResponse>()
                .ForMember(dest => dest.TotalAlimentos,
                    opt => opt.MapFrom(src => src.Alimentos != null
                        ? src.Alimentos.Count(a => a.Activo && !a.Eliminado)
                        : 0));
        }
    }
}
