using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class AlimentoMappingProfile : Profile
    {
        public AlimentoMappingProfile()
        {

            CreateMap<AlimentoDto.Create, Alimento>();

            CreateMap<AlimentoDto.Update, Alimento>();

            CreateMap<Alimento, AlimentoDto.Response>()
                .ForMember(dest => dest.CategoriaNombre,
                    opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty));


            CreateMap<CategoriaDto.Create, CategoriaAlimento>();

            CreateMap<CategoriaDto.Update, CategoriaAlimento>();

            CreateMap<CategoriaAlimento, CategoriaDto.Response>()
                .ForMember(dest => dest.TotalAlimentos,
                    opt => opt.MapFrom(src => src.Alimentos != null
                        ? src.Alimentos.Count(a => a.Activo && !a.Eliminado)
                        : 0));
        }
    }
}