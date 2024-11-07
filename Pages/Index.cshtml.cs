using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace Trabajo_UBU.Pages
{
    public class IndexModel : PageModel
    {
        // Propiedades enlazadas para recibir el email y la contrase�a de inicio de sesi�n
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        // Mensajes de retroalimentaci�n para el usuario
        public string Message { get; set; } // Mensaje de error de inicio de sesi�n
        public string RequestMessage { get; set; } // Mensaje para solicitudes de cuenta

        // Propiedades enlazadas para la solicitud de nueva cuenta
        [BindProperty]
        public string NewEmail { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        // Dependencia de la capa de datos para la gesti�n de usuarios y solicitudes de cuenta
        private readonly ICapaDatos _dbGenTree;

        // Constructor que recibe una instancia de ICapaDatos
        public IndexModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        public void OnGet() { } // M�todo para inicializar la p�gina en una solicitud GET

        // M�todo para el manejo del inicio de sesi�n
        public async Task<IActionResult> OnPostLogin()
        {
            // Verifica si el usuario y la contrase�a son v�lidos
            if (_dbGenTree.ValidateUser(Email, Password))
            {
                string role = _dbGenTree.GetUserRole(Email); // Obtiene el rol del usuario

                // Crea las credenciales del usuario para la autenticaci�n
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim(ClaimTypes.Role, role)
                };

                // Crea la identidad de las credenciales y el cookie de autenticaci�n
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Mantiene al usuario autenticado incluso despu�s de cerrar el navegador
                };

                // Firma al usuario para establecer la sesi�n de autenticaci�n
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirige a la p�gina de perfil tras el inicio de sesi�n exitoso
                return RedirectToPage("/Profile", new { userName = Email, role });
            }

            // Mensaje de error si las credenciales son incorrectas
            Message = "Las credenciales no son v�lidas.";
            return Page();
        }

        // M�todo para manejar las solicitudes de creaci�n de nueva cuenta
        public IActionResult OnPostRequestAccount()
        {
            // Validaci�n b�sica de los campos de email y contrase�a
            if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                RequestMessage = "No pueden quedar espacios en blanco.";
                return Page();
            }

            // Verifica los requisitos de la contrase�a (m�nimo 12 caracteres, solo letras y n�meros)
            var passwordCriteria = new System.Text.RegularExpressions.Regex(@"^[A-Za-z\d]{12,}$");

            if (!passwordCriteria.IsMatch(NewPassword))
            {
                RequestMessage = "La contrase�a debe de tener al menos 12 caracteres y contener �nicamente letras y n�meros.";
                return Page();
            }

            // Verifica que la contrase�a y su confirmaci�n coincidan
            if (NewPassword != ConfirmPassword)
            {
                RequestMessage = "Las contrase�as no coinciden.";
                return Page();
            }

            // Agrega la solicitud de nueva cuenta a la lista de pendientes
            _dbGenTree.AddAccountRequest(NewEmail, NewPassword);

            // Mensaje de confirmaci�n para el usuario tras solicitar la cuenta
            RequestMessage = "Gracias por solicitar una cuenta. Pronto ser�s contactado.";

            return Page(); // Recarga la p�gina mostrando el mensaje de confirmaci�n
        }
    }
}
