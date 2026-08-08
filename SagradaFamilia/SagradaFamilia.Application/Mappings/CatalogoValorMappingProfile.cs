using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class CatalogoValorMappingProfile : Profile
    {
        public CatalogoValorMappingProfile()
        {
            CreateMap<CatalogoValor, CatalogoValorDto.Response>();
            CreateMap<CatalogoValorDto.Create, CatalogoValor>();
            CreateMap<CatalogoValorDto.Update, CatalogoValor>();
        }
    }
}
