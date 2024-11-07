using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    public class WarehouseModel : PageModel
    {
        private readonly ICapaDatos _dbGenTree; // Dependencia para la capa de datos

        // Dimensiones de la matriz que representa el almacén
        public int Rows => 5;    // Número de filas del almacén
        public int Columns => 5; // Número de columnas del almacén 

        public List<string> OccupiedSlots { get; set; } // Lista de espacios ocupados en el almacén
        public string UserSlot { get; set; } // Espacio asignado al usuario actual

        // Constructor que inicializa la clase con la capa de datos
        public WarehouseModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
            OccupiedSlots = new List<string>(); // Inicializa la lista de espacios ocupados
            UserSlot = string.Empty; // Inicializa el espacio del usuario como vacío
        }

        public IActionResult OnGet()
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(userEmail))
            {
                // Si el usuario no está autenticado, redirigir a la página de inicio de sesión
                return RedirectToPage("/Index");
            }

            // Obtener el espacio asignado al usuario y la lista de espacios ocupados en el almacén
            UserSlot = _dbGenTree.GetUserSlot(userEmail); // Obtiene el espacio asignado solo al usuario logueado
            OccupiedSlots = _dbGenTree.GetOccupiedSlots(); // Obtiene todos los espacios ocupados

            return Page(); // Renderizar la página
        }

        public IActionResult OnPost(string SelectedSlot)
        {
            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(userEmail))
            {
                // Si el usuario no está autenticado, redirigir a la página de inicio de sesión
                return RedirectToPage("/Index");
            }

            // Intentar asignar el espacio seleccionado al usuario actual
            bool success = _dbGenTree.AssignSlot(userEmail, SelectedSlot);

            if (success)
            {
                // Si la asignación fue exitosa, actualizar el espacio del usuario
                UserSlot = SelectedSlot;
                TempData["ConfirmationMessage"] = "¡Su selección de espacio ha sido guardada exitosamente!";
            }
            else
            {
                // Mensaje de error en caso de que el espacio ya esté ocupado
                TempData["ErrorMessage"] = "No se pudo guardar la selección de espacio. Es posible que ya esté ocupado.";
            }

            return RedirectToPage(); // Redirigir de nuevo a la página después de la acción
        }
    }
}
