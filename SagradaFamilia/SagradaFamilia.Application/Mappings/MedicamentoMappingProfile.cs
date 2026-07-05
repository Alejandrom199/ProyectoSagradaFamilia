using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class MedicamentoMappingProfile : Profile
    {
        public MedicamentoMappingProfile()
        {
            CreateMap<MedicamentoDto.Item, Medicamento>();
            CreateMap<Medicamento, MedicamentoDto.Response>();
        }
    }
}
