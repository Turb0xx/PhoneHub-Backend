using PhoneHub.Core.Entities;
using System;
using System.Collections.Generic;

namespace PhoneHub.Core.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        // El producto que se está vendiendo (Celular/Accesorio)
        public int ProductId { get; set; }

        // El empleado que realizó la transacción (Para el cierre de caja)
        public int UserId { get; set; }

        public int Quantity { get; set; } // Cuántas unidades se vendieron

        public decimal TotalAmount { get; set; } // Monto total de la venta (siempre decimal para dinero)

        public DateTime Date { get; set; } // Fecha y hora de la venta

        public bool IsActive { get; set; } // Para anular facturas sin borrarlas de la DB

        // Relaciones de navegación (Clean Architecture)
        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}