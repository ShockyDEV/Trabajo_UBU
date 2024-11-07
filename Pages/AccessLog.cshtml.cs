using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    public class AccessLogModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        public List<LogEntry> AccessLog { get; set; }

        private readonly ICapaDatos _dbGenTree;

        public AccessLogModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        public void OnGet()
        {
            AccessLog = _dbGenTree.GetAccessLog();
        }
    }
}
