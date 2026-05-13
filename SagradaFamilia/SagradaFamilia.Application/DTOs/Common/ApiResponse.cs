namespace SagradaFamilia.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Operación exitosa") =>
            new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string error) =>
            new() { Success = false, Message = error, Errors = new List<string> { error } };

        public static ApiResponse<T> Fail(List<string> errors) =>
            new() { Success = false, Message = "Errores de validación", Errors = errors };
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse OkNoData(string message = "Operación exitosa") =>
            new() { Success = true, Message = message };
    }
}