using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Application.DTOs.Common
{
    public class PagedResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public static PagedResponse<T> Ok(
            IEnumerable<T> data,
            int totalItems,
            int page,
            int pageSize) =>
            new()
            {
                Success = true,
                Message = "Operación exitosa",
                Data = data,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
    }
}
