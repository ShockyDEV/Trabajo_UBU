using System.Collections.Generic;
using System.Linq;
using Trabajo_UBU;
using System;

public class DBGenTree : ICapaDatos
{
    // Cambiar a Dictionary<string, User> para que cada valor sea una instancia de User
    private static readonly Dictionary<string, User> Users = new Dictionary<string, User>
    {
        { "admin@school.com", new User { Email = "admin@school.com", PasswordHash = Bcrypt.HashPassword("admin123"), Role = "Admin" } },
        { "user@school.com", new User { Email = "user@school.com", PasswordHash = Bcrypt.HashPassword("user123"), Role = "User" } }
    };

    private static readonly List<LogEntry> AccessLog = new List<LogEntry>();
    private static readonly List<AccountRequest> AccountRequests = new List<AccountRequest>();

    public bool ValidateUser(string email, string password)
    {
        bool success = false;

        if (Users.TryGetValue(email, out var userInfo))
        {
            success = Bcrypt.VerifyPassword(password, userInfo.PasswordHash);
        }

        // Log the login attempt
        AccessLog.Add(new LogEntry
        {
            Timestamp = DateTime.Now,
            UserName = email,
            Success = success
        });

        return success;
    }

    public string GetUserRole(string email)
    {
        return Users.ContainsKey(email) ? Users[email].Role : "User";
    }

    public List<LogEntry> GetAccessLog()
    {
        return AccessLog;
    }

    public void AddAccountRequest(string email, string password)
    {
        AccountRequests.Add(new AccountRequest
        {
            Email = email,
            PasswordHash = Bcrypt.HashPassword(password),
            Status = "Pending"
        });
    }

    public List<AccountRequest> GetAllAccountRequests()
    {
        return AccountRequests;
    }

    public void ApproveAccountRequest(string email)
    {
        var request = AccountRequests.Find(r => r.Email == email);
        if (request != null)
        {
            Users[email] = new User
            {
                Email = email,
                PasswordHash = request.PasswordHash,
                Role = "User"
            };
            request.Status = "Approved";
            request.ApprovalDate = DateTime.Now;
        }
    }

    public void RejectAccountRequest(string email)
    {
        var request = AccountRequests.Find(r => r.Email == email);
        if (request != null)
        {
            request.Status = "Rejected";
            request.ApprovalDate = DateTime.Now;
        }
    }
    public List<string> GetOccupiedSlots()
    {
        // Devuelve los huecos ocupados por los usuarios
        return Users.Values.Where(u => !string.IsNullOrEmpty(u.SelectedSlot)).Select(u => u.SelectedSlot).ToList();
    }

    public string GetUserSlot(string email)
    {
        return Users.ContainsKey(email) ? Users[email].SelectedSlot : null;
    }

    public bool AssignSlot(string email, string slot)
    {
        // Check if the slot is already occupied by another user
        if (Users.Values.Any(u => u.SelectedSlot == slot))
        {
            return false; // Slot is already occupied
        }

        // Assign the slot to the user if they exist in the dictionary
        if (Users.ContainsKey(email) && string.IsNullOrEmpty(Users[email].SelectedSlot))
        {
            Users[email].SelectedSlot = slot;
            return true;
        }

        return false; // User does not exist or already has a slot
    }
}

public interface ICapaDatos
{
    bool ValidateUser(string email, string password);
    string GetUserRole(string email);
    List<LogEntry> GetAccessLog();
    void AddAccountRequest(string email, string password);
    List<AccountRequest> GetAllAccountRequests();
    void ApproveAccountRequest(string email);
    void RejectAccountRequest(string email);

    List<string> GetOccupiedSlots();
    string GetUserSlot(string email);
    bool AssignSlot(string email, string slot);
}


internal class User
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public string SelectedSlot { get; set; } // Almacena el hueco seleccionado
}

public class AccountRequest
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Status { get; set; }
    public DateTime? ApprovalDate { get; set; }
}
