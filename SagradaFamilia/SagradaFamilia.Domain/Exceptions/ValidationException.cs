namespace SagradaFamilia.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("Se han producido uno o más errores de validación.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : this()
        {
            Errors = errors;
        }
    }
}