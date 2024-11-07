using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace Trabajo_UBU.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string Message { get; set; }
        public string RequestMessage { get; set; }

        [BindProperty]
        public string NewEmail { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        private readonly ICapaDatos _dbGenTree;

        public IndexModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostLogin()
        {
            if (_dbGenTree.ValidateUser(Email, Password))
            {
                string role = _dbGenTree.GetUserRole(Email);

                // Create user claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Email),
            new Claim(ClaimTypes.Role, role)
        };

                // Create the claims identity and the authentication cookie
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Keeps the user logged in even after closing the browser
                };

                // Sign in the user
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect to the profile page after successful login
                return RedirectToPage("/Profile", new { userName = Email, role });
            }

            // Display error message if credentials are invalid
            Message = "Las credenciales no son válidas.";
            return Page();
        }


        public IActionResult OnPostRequestAccount()
        {
            // Basic validation for email and password
            if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                RequestMessage = "No pueden quedar espacios en blanco.";
                return Page();
            }

            // Password requirements check
            var passwordCriteria = new System.Text.RegularExpressions.Regex(@"^[A-Za-z\d]{12,}$");

            if (!passwordCriteria.IsMatch(NewPassword))
            {
                RequestMessage = "La contraseña debe de tener al menos 12 carácteres y contener únicamente letras y números.";
                return Page();
            }

            // Confirm password match
            if (NewPassword != ConfirmPassword)
            {
                RequestMessage = "Las contraseñas no coinciden.";
                return Page();
            }

            // Add new account request to pending list
            _dbGenTree.AddAccountRequest(NewEmail, NewPassword);

            // Set a confirmation message for the user
            RequestMessage = "Gracias por solicitar una cuenta. Pronto serás contactado.";

            return Page(); // Reload the page with the message
        }
    }
}


