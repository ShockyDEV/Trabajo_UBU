using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Trabajo_UBU.Pages
{
    public class WarehouseModel : PageModel
    {
        private readonly ICapaDatos _dbGenTree;

        public int Rows => 5;    // Define the dimensions of the warehouse grid
        public int Columns => 5; // Adjust as needed

        public List<string> OccupiedSlots { get; set; }
        public string UserSlot { get; set; }

        public WarehouseModel(ICapaDatos dbGenTree)
        {
            _dbGenTree = dbGenTree;
            OccupiedSlots = new List<string>();
            UserSlot = string.Empty;
        }

        public IActionResult OnGet()
        {
            var userEmail = User.Identity?.Name?.ToLower(); // Get current user's email
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login page if not authenticated
                return RedirectToPage("/Index");
            }

            // Retrieve the specific user’s slot and the occupied slots
            UserSlot = _dbGenTree.GetUserSlot(userEmail); // Gets only the slot for the logged-in user
            OccupiedSlots = _dbGenTree.GetOccupiedSlots(); // Gets all occupied slots

            return Page();
        }

        public IActionResult OnPost(string SelectedSlot)
        {
            var userEmail = User.Identity?.Name?.ToLower(); // Get current user's email
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login page if not authenticated
                return RedirectToPage("/Index");
            }

            // Try to assign the selected slot to the logged-in user
            bool success = _dbGenTree.AssignSlot(userEmail, SelectedSlot);

            if (success)
            {
                UserSlot = SelectedSlot; // Update UserSlot after successful assignment
                TempData["ConfirmationMessage"] = "Your slot selection has been saved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Slot selection failed. It may already be occupied.";
            }

            return RedirectToPage();
        }
    }
}
