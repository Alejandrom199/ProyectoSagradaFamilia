using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Mappings
{
    public class LogSistemaMappingProfile : Profile
    {
        public LogSistemaMappingProfile()
        {
            CreateMap<LogSistema, LogSistemaDto.Response>();
        }
    }
}
