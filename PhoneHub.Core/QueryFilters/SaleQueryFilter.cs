using Swashbuckle.AspNetCore.Annotations;

namespace PhoneHub.Core.QueryFilters
{
    public class SaleQueryFilter : PaginationQueryFilter
    {
        [SwaggerSchema("Filtrar ventas por ID de vendedor.")]
        public int? UserId { get; set; }

        [SwaggerSchema("Filtrar ventas por ID de producto.")]
        public int? ProductId { get; set; }
    }
}
