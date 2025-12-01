using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Theater.Services
{
    public class PasswordHash
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Преобразуем строку в байты
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // Вычисляем хеш
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Преобразуем байты обратно в строку (для простоты используем Base64)
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }

    }
}
