using Swashbuckle.AspNetCore.Annotations;

namespace PhoneHub.Core.QueryFilters
{
    public class ProductQueryFilter : PaginationQueryFilter
    {
        [SwaggerSchema("Filtrar por marca del producto.")]
        public string? Brand { get; set; }

        [SwaggerSchema("Filtrar por modelo del producto.")]
        public string? Model { get; set; }

        [SwaggerSchema("Precio máximo para filtrar.")]
        public decimal? MaxPrice { get; set; }

        [SwaggerSchema("Si es true, solo retorna productos con stock mayor a cero.")]
        public bool? OnlyAvailable { get; set; }
    }
}
