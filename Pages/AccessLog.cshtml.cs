using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    // Modelo de p�gina para mostrar el log de accesos
    public class AccessLogModel : PageModel
    {
        // Propiedad enlazada para recibir el nombre del usuario a trav�s de la URL
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        // Lista que contiene los registros de acceso obtenidos de la base de datos
        public List<LogEntry> AccessLog { get; set; }

        // Dependencia de la capa de datos, usada para interactuar con los datos del log de accesos
        private readonly ICapaDatos _dbGenTree;

        // Constructor que recibe una instancia de ICapaDatos para inicializar la capa de datos
        public AccessLogModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        // M�todo que se ejecuta al cargar la p�gina y obtiene el log de accesos de la base de datos
        public void OnGet()
        {
            AccessLog = _dbGenTree.GetAccessLog(); // Obtiene y almacena el log de accesos
        }
    }
}
