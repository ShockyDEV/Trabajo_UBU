using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace Trabajo_UBU.Pages
{
    public class IndexModel : PageModel
    {
        // Propiedades enlazadas para recibir el email y la contraseña de inicio de sesión
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        // Mensajes de retroalimentación para el usuario
        public string Message { get; set; } // Mensaje de error de inicio de sesión
        public string RequestMessage { get; set; } // Mensaje para solicitudes de cuenta

        // Propiedades enlazadas para la solicitud de nueva cuenta
        [BindProperty]
        public string NewEmail { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        // Dependencia de la capa de datos para la gestión de usuarios y solicitudes de cuenta
        private readonly ICapaDatos _dbGenTree;

        // Constructor que recibe una instancia de ICapaDatos
        public IndexModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        public void OnGet() { } // Método para inicializar la página en una solicitud GET

        // Método para el manejo del inicio de sesión
        public async Task<IActionResult> OnPostLogin()
        {
            // Verifica si el usuario y la contraseña son válidos
            if (_dbGenTree.ValidateUser(Email, Password))
            {
                string role = _dbGenTree.GetUserRole(Email); // Obtiene el rol del usuario

                // Crea las credenciales del usuario para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim(ClaimTypes.Role, role)
                };

                // Crea la identidad de las credenciales y el cookie de autenticación
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Mantiene al usuario autenticado incluso después de cerrar el navegador
                };

                // Firma al usuario para establecer la sesión de autenticación
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirige a la página de perfil tras el inicio de sesión exitoso
                return RedirectToPage("/Profile", new { userName = Email, role });
            }

            // Mensaje de error si las credenciales son incorrectas
            Message = "Las credenciales no son válidas.";
            return Page();
        }

        // Método para manejar las solicitudes de creación de nueva cuenta
        public IActionResult OnPostRequestAccount()
        {
            // Validación básica de los campos de email y contraseña
            if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                RequestMessage = "No pueden quedar espacios en blanco.";
                return Page();
            }

            // Verifica los requisitos de la contraseña (mínimo 12 caracteres, solo letras y números)
            var passwordCriteria = new System.Text.RegularExpressions.Regex(@"^[A-Za-z\d]{12,}$");

            if (!passwordCriteria.IsMatch(NewPassword))
            {
                RequestMessage = "La contraseña debe de tener al menos 12 caracteres y contener únicamente letras y números.";
                return Page();
            }

            // Verifica que la contraseña y su confirmación coincidan
            if (NewPassword != ConfirmPassword)
            {
                RequestMessage = "Las contraseñas no coinciden.";
                return Page();
            }

            // Agrega la solicitud de nueva cuenta a la lista de pendientes
            _dbGenTree.AddAccountRequest(NewEmail, NewPassword);

            // Mensaje de confirmación para el usuario tras solicitar la cuenta
            RequestMessage = "Gracias por solicitar una cuenta. Pronto serás contactado.";

            return Page(); // Recarga la página mostrando el mensaje de confirmación
        }
    }
}
