using PhoneHub.Core.Entities;
using System;
using System.Collections.Generic;

namespace PhoneHub.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        // Datos básicos del equipo (Requerimiento 4)
        public string Brand { get; set; } = null!; // Marca (ej. Apple, Samsung)

        public string Model { get; set; } = null!; // Modelo (ej. iPhone 15 Pro Max)

        public string? Description { get; set; }

        // Control de Dinero y Stock (Requerimiento 1 y 3)
        public decimal Price { get; set; } // Precio de venta al público

        public int Stock { get; set; } // Cantidad disponible en tienda

        public string? Image { get; set; } // Opcional: URL de la foto del equipo

        public DateTime CreatedAt { get; set; } // Fecha en que se registró el producto

        // Relación: Un producto puede estar en muchas ventas (detalles de factura)
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}