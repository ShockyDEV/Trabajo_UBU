using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace Trabajo_UBU.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Role { get; set; }


        public List<LogEntry> AccessLog { get; set; }

        private readonly ICapaDatos _dbGenTree;

        public ProfileModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
        }

        public void OnGet()
        {
            if (Role == "Admin")
            {
                AccessLog = _dbGenTree.GetAccessLog();
            }
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToPage("/Index");
        }
    }
}
