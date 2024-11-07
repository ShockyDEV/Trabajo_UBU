using System;
using System.Security.Cryptography;
using System.Text;

public static class Bcrypt
{
    // Tamaño del salt en bytes
    private const int SaltSize = 16;
    // Factor de trabajo, define la cantidad de iteraciones para el hash (mayor valor implica más seguridad y más tiempo de procesamiento)
    private const int WorkFactor = 100000;

    // Método para generar un hash de la contraseña con un nuevo salt
    public static string HashPassword(string password)
    {
        var salt = GenerateSalt(); // Genera un salt aleatorio
        return HashPassword(password, salt); // Genera y devuelve el hash de la contraseña con el salt
    }

    // Método privado para generar un hash de la contraseña utilizando un salt específico
    private static string HashPassword(string password, byte[] salt)
    {
        // Utiliza el algoritmo PBKDF2 con SHA-256 para derivar una clave a partir de la contraseña
        using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, WorkFactor, HashAlgorithmName.SHA256))
        {
            var hash = rfc2898DeriveBytes.GetBytes(32); // Genera un hash de 32 bytes usando SHA-256
            var hashBytes = new byte[SaltSize + hash.Length]; // Array para almacenar el salt y el hash combinados
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize); // Copia el salt en los primeros bytes
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, hash.Length); // Copia el hash después del salt
            return Convert.ToBase64String(hashBytes); // Convierte el resultado a una cadena en Base64
        }
    }

    // Método para verificar si una contraseña coincide con un hash almacenado
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword); // Convierte el hash en Base64 a bytes
        var salt = new byte[SaltSize];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize); // Extrae el salt de los primeros bytes
        var hash = HashPassword(password, salt); // Genera el hash de la contraseña ingresada usando el salt extraído
        return hash == hashedPassword; // Compara el hash generado con el hash almacenado
    }

    // Método privado para generar un salt aleatorio
    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt); // Llena el array con bytes aleatorios
        return salt; // Devuelve el salt generado
    }
}
