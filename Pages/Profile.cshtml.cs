using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace Trabajo_UBU.Pages
{
    public class ProfileModel : PageModel
    {
        // Propiedad enlazada para recibir el nombre de usuario a través de la URL
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        // Propiedad enlazada para recibir el rol del usuario a través de la URL
        [BindProperty(SupportsGet = true)]
        public string Role { get; set; }

        // Lista para almacenar el log de accesos, visible solo para el administrador
        public List<LogEntry> AccessLog { get; set; }

        // Dependencia para la capa de datos, se utiliza para interactuar con la base de datos o repositorio
        private readonly ICapaDatos _dbGenTree;

        // Constructor que recibe una instancia de ICapaDatos, usada para obtener datos de la base
        public ProfileModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        // Método que se ejecuta al cargar la página
        public void OnGet()
        {
            // Si el usuario tiene rol de administrador, obtiene el log de accesos
            if (Role == "Admin")
            {
                AccessLog = _dbGenTree.GetAccessLog();
            }
        }

        // Método que se ejecuta al hacer clic en el botón de cerrar sesión
        public async Task<IActionResult> OnPostLogout()
        {
            // Cierra la sesión del usuario autenticado usando CookieAuth
            await HttpContext.SignOutAsync("CookieAuth");
            // Redirige a la página principal después de cerrar sesión
            return RedirectToPage("/Index");
        }
    }
}
