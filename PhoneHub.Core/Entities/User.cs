using PhoneHub.Core.Entities;
using System;
using System.Collections.Generic;

namespace PhoneHub.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        // Agregamos contraseña para el Login (Requerimiento 2)
        public string Password { get; set; } = null!;

        // Para diferenciar entre "Administrador", "Cajero", etc.
        public string Role { get; set; } = null!;

        public string? Telephone { get; set; }

        // Cambiamos ulong por bool (true si el empleado sigue trabajando, false si fue despedido/suspendido)
        public bool IsActive { get; set; }

        // Relación: Un empleado (cajero) procesa muchas ventas (Requerimiento 3)
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}