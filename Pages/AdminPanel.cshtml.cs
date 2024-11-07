using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

    namespace Trabajo_UBU.Pages
    {
        public class AdminPanelModel : PageModel
        {
            [BindProperty(SupportsGet = true)]
            public string UserName { get; set; } 

            public List<AccountRequest> PendingRequests { get; private set; }

            private readonly ICapaDatos _dbGenTree;

            public AdminPanelModel(ICapaDatos dbGenTree)
            {
                _dbGenTree = dbGenTree;
            }

            public void OnGet()
            {
                PendingRequests = _dbGenTree.GetAllAccountRequests();
            }

            public IActionResult OnPostApprove(string email)
            {
                _dbGenTree.ApproveAccountRequest(email);
                return RedirectToPage();
            }

            public IActionResult OnPostReject(string email)
            {
                _dbGenTree.RejectAccountRequest(email);
                return RedirectToPage();
            }
        }
    }

