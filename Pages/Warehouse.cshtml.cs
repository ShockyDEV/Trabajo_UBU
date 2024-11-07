using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    public class WarehouseModel : PageModel
    {
        private readonly ICapaDatos _dbGenTree; // Dependencia para la capa de datos

        // Dimensiones de la matriz que representa el almac�n
        public int Rows => 5;    // N�mero de filas del almac�n
        public int Columns => 5; // N�mero de columnas del almac�n 

        public List<string> OccupiedSlots { get; set; } // Lista de espacios ocupados en el almac�n
        public string UserSlot { get; set; } // Espacio asignado al usuario actual

        // Constructor que inicializa la clase con la capa de datos
        public WarehouseModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
            OccupiedSlots = new List<string>(); // Inicializa la lista de espacios ocupados
            UserSlot = string.Empty; // Inicializa el espacio del usuario como vac�o
        }

        public IActionResult OnGet()
        {
            // Obtener el correo electr�nico del usuario actual
            var userEmail = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(userEmail))
            {
                // Si el usuario no est� autenticado, redirigir a la p�gina de inicio de sesi�n
                return RedirectToPage("/Index");
            }

            // Obtener el espacio asignado al usuario y la lista de espacios ocupados en el almac�n
            UserSlot = _dbGenTree.GetUserSlot(userEmail); // Obtiene el espacio asignado solo al usuario logueado
            OccupiedSlots = _dbGenTree.GetOccupiedSlots(); // Obtiene todos los espacios ocupados

            return Page(); // Renderizar la p�gina
        }

        public IActionResult OnPost(string SelectedSlot)
        {
            // Obtener el correo electr�nico del usuario actual
            var userEmail = User.Identity?.Name?.ToLower();
            if (string.IsNullOrEmpty(userEmail))
            {
                // Si el usuario no est� autenticado, redirigir a la p�gina de inicio de sesi�n
                return RedirectToPage("/Index");
            }

            // Intentar asignar el espacio seleccionado al usuario actual
            bool success = _dbGenTree.AssignSlot(userEmail, SelectedSlot);

            if (success)
            {
                // Si la asignaci�n fue exitosa, actualizar el espacio del usuario
                UserSlot = SelectedSlot;
                TempData["ConfirmationMessage"] = "�Su selecci�n de espacio ha sido guardada exitosamente!";
            }
            else
            {
                // Mensaje de error en caso de que el espacio ya est� ocupado
                TempData["ErrorMessage"] = "No se pudo guardar la selecci�n de espacio. Es posible que ya est� ocupado.";
            }

            return RedirectToPage(); // Redirigir de nuevo a la p�gina despu�s de la acci�n
        }
    }
}
