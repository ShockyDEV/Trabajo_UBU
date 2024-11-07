using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    // Modelo de la p�gina para el panel de administraci�n
    public class AdminPanelModel : PageModel
    {
        // Propiedad enlazada para recibir el nombre del usuario administrador a trav�s de la URL
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        // Lista que contiene las solicitudes de cuenta pendientes
        public List<AccountRequest> PendingRequests { get; private set; }

        // Dependencia de la capa de datos, usada para interactuar con la base de datos de solicitudes de cuentas
        private readonly ICapaDatos _dbGenTree;

        // Constructor que recibe una instancia de ICapaDatos para inicializar la capa de datos
        public AdminPanelModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        // M�todo que se ejecuta al cargar la p�gina, obtiene todas las solicitudes de cuenta pendientes
        public void OnGet()
        {
            PendingRequests = _dbGenTree.GetAllAccountRequests(); // Obtiene todas las solicitudes de cuenta
        }

        // M�todo para aprobar una solicitud de cuenta espec�fica, identificado por el email
        public IActionResult OnPostApprove(string email)
        {
            _dbGenTree.ApproveAccountRequest(email); // Aprueba la solicitud de cuenta
            return RedirectToPage(); // Recarga la p�gina despu�s de la aprobaci�n
        }

        // M�todo para rechazar una solicitud de cuenta espec�fica, identificado por el email
        public IActionResult OnPostReject(string email)
        {
            _dbGenTree.RejectAccountRequest(email); // Rechaza la solicitud de cuenta
            return RedirectToPage(); // Recarga la p�gina despu�s del rechazo
        }
    }
}
