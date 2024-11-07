using System.Collections.Generic;
using System.Linq;
using Trabajo_UBU;
using System;

public class DBGenTree : ICapaDatos
{
    // Diccionario que almacena los usuarios registrados en el sistema, usando su email como clave
    private static readonly Dictionary<string, User> Users = new Dictionary<string, User>
    {
        // Usuario administrador con acceso total al sistema
        { "admin@school.com", new User { Email = "admin@school.com", PasswordHash = Bcrypt.HashPassword("admin123"), Role = "Admin" } },
        // Usuario regular con permisos limitados
        { "user@school.com", new User { Email = "user@school.com", PasswordHash = Bcrypt.HashPassword("user123"), Role = "User" } }
    };

    // Lista que almacena los intentos de acceso al sistema
    private static readonly List<LogEntry> AccessLog = new List<LogEntry>();
    // Lista de solicitudes de creación de cuenta de usuario
    private static readonly List<AccountRequest> AccountRequests = new List<AccountRequest>();

    // Método para validar un usuario a partir de su email y contraseña
    public bool ValidateUser(string email, string password)
    {
        bool success = false;

        // Verifica si el email existe en el diccionario de usuarios
        if (Users.TryGetValue(email, out var userInfo))
        {
            // Compara la contraseña ingresada con el hash almacenado
            success = Bcrypt.VerifyPassword(password, userInfo.PasswordHash);
        }

        // Registra el intento de inicio de sesión en el log
        AccessLog.Add(new LogEntry
        {
            Timestamp = DateTime.Now,
            UserName = email,
            Success = success
        });

        return success;
    }

    // Obtiene el rol de un usuario según su email
    public string GetUserRole(string email)
    {
        return Users.ContainsKey(email) ? Users[email].Role : "User";
    }

    // Devuelve el log de accesos completo
    public List<LogEntry> GetAccessLog()
    {
        return AccessLog;
    }

    // Añade una solicitud de cuenta con email y contraseña
    public void AddAccountRequest(string email, string password)
    {
        AccountRequests.Add(new AccountRequest
        {
            Email = email,
            PasswordHash = Bcrypt.HashPassword(password),
            Status = "Pending" // Estado inicial de la solicitud
        });
    }

    // Devuelve todas las solicitudes de cuenta pendientes
    public List<AccountRequest> GetAllAccountRequests()
    {
        return AccountRequests;
    }

    // Aprueba una solicitud de cuenta y crea un nuevo usuario en el sistema
    public void ApproveAccountRequest(string email)
    {
        var request = AccountRequests.Find(r => r.Email == email);
        if (request != null)
        {
            // Crea el usuario a partir de la solicitud aprobada
            Users[email] = new User
            {
                Email = email,
                PasswordHash = request.PasswordHash,
                Role = "User"
            };
            request.Status = "Approved"; // Cambia el estado a 'Aprobado'
            request.ApprovalDate = DateTime.Now;
        }
    }

    // Rechaza una solicitud de cuenta
    public void RejectAccountRequest(string email)
    {
        var request = AccountRequests.Find(r => r.Email == email);
        if (request != null)
        {
            request.Status = "Rejected"; // Cambia el estado a 'Rechazado'
            request.ApprovalDate = DateTime.Now;
        }
    }

    // Devuelve una lista con los huecos ocupados por los usuarios
    public List<string> GetOccupiedSlots()
    {
        return Users.Values.Where(u => !string.IsNullOrEmpty(u.SelectedSlot)).Select(u => u.SelectedSlot).ToList();
    }

    // Obtiene el hueco asignado a un usuario específico según su email
    public string GetUserSlot(string email)
    {
        return Users.ContainsKey(email) ? Users[email].SelectedSlot : null;
    }

    // Asigna un hueco a un usuario si está disponible
    public bool AssignSlot(string email, string slot)
    {
        // Verifica si el hueco ya está ocupado por otro usuario
        if (Users.Values.Any(u => u.SelectedSlot == slot))
        {
            return false; // El hueco ya está ocupado
        }

        // Asigna el hueco al usuario si existe y no tiene ya uno asignado
        if (Users.ContainsKey(email) && string.IsNullOrEmpty(Users[email].SelectedSlot))
        {
            Users[email].SelectedSlot = slot;
            return true;
        }

        return false; // El usuario no existe o ya tiene un hueco asignado
    }
}

// Interfaz que define los métodos para manipulación de datos
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

// Clase que representa a un usuario del sistema
internal class User
{
    public string Email { get; set; } // Email del usuario
    public string PasswordHash { get; set; } // Hash de la contraseña del usuario
    public string Role { get; set; } // Rol del usuario (Admin o User)
    public string SelectedSlot { get; set; } // Hueco seleccionado por el usuario
}

// Clase que representa una solicitud de creación de cuenta de usuario
public class AccountRequest
{
    public string Email { get; set; } // Email de la solicitud
    public string PasswordHash { get; set; } // Hash de la contraseña
    public string Status { get; set; } // Estado de la solicitud (Pendiente, Aprobado o Rechazado)
    public DateTime? ApprovalDate { get; set; } // Fecha de aprobación o rechazo
}
