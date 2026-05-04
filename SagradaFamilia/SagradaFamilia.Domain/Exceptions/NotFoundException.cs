namespace SagradaFamilia.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entity, int id)
            : base($"{entity} con Id {id} no fue encontrado.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
