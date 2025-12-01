using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Theater.Services
{
    public class NetHelper
    {
        private static readonly string CacheFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");

        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private static string GetHashString(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16).ToLower();
        }

        public static async Task<string> DownloadAndCacheImageAsync(string imageUrl)
        {
            if (!Directory.Exists(CacheFolder)) Directory.CreateDirectory(CacheFolder);
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            string fileName = imageUrl;
            string localPath = Path.Combine(CacheFolder, $"{GetHashString(fileName)}.jpg");

            // Если уже есть — сразу возвращаем
            if (File.Exists(localPath))
                return localPath;

            try
            {
                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                File.WriteAllBytes(localPath, imageBytes);
                return localPath;
            }
            catch (Exception ex)
            {
                // Логируем ошибку (можно заменить на ваш логгер)
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения {imageUrl}: {ex.Message}");
                return null;
            }
        }

        public static async Task<BitmapImage> GetBitmapAsync(string imageUrl)
        {
            string localPath = await DownloadAndCacheImageAsync(imageUrl);

            if (localPath == null || !File.Exists(localPath))
                return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(localPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // важно для потокобезопасности
            bitmap.EndInit();
            bitmap.Freeze(); // делаем потокобезопасным для UI

            return bitmap;
        }
    }
}
