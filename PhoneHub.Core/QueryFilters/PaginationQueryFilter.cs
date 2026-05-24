using Swashbuckle.AspNetCore.Annotations;

namespace PhoneHub.Core.QueryFilters
{
    public class PaginationQueryFilter
    {
        /// <summary>
        /// Número de página (inicia en 1)
        /// </summary>
        [SwaggerSchema("Número de página a mostrar", Nullable = false)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Cantidad de registros por página
        /// </summary>
        [SwaggerSchema("Cantidad de registros por página", Nullable = false)]
        public int PageSize { get; set; } = 10;
    }
}
