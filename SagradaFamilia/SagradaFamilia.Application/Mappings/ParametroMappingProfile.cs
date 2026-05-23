using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class ParametroMappingProfile : Profile
    {
        public ParametroMappingProfile()
        {
            CreateMap<ParametroSistema, ParametroDto.Response>();
            CreateMap<ParametroDto.Create, ParametroSistema>();
            CreateMap<ParametroDto.Update, ParametroSistema>();
        }
    }
}
