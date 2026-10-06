using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CambioEstatus
{
    public static class ConfigManager
    {
        private static readonly string ConfigFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "CambioEstatus");

        private static readonly string ConfigPath = Path.Combine(ConfigFolder, "config.dat");

        public static void Guardar(SapConfig config)
        {
            Directory.CreateDirectory(ConfigFolder);

            string json = JsonSerializer.Serialize(config);
            byte[] plainBytes = Encoding.UTF8.GetBytes(json);

            // Cifrado atado al usuario actual de Windows
            byte[] encrypted = ProtectedData.Protect(
                plainBytes,
                optionalEntropy: null,
                scope: DataProtectionScope.CurrentUser);

            File.WriteAllBytes(ConfigPath, encrypted);
        }

        public static SapConfig? Cargar()
        {
            if (!File.Exists(ConfigPath)) return null;

            try
            {
                byte[] encrypted = File.ReadAllBytes(ConfigPath);
                byte[] plainBytes = ProtectedData.Unprotect(
                    encrypted,
                    optionalEntropy: null,
                    scope: DataProtectionScope.CurrentUser);

                string json = Encoding.UTF8.GetString(plainBytes);
                return JsonSerializer.Deserialize<SapConfig>(json);
            }
            catch
            {
                return null;
            }
        }

        public static bool Existe()
        {
            return File.Exists(ConfigPath);
        }

        public static void Eliminar()
        {
            if (File.Exists(ConfigPath))
                File.Delete(ConfigPath);
        }
    }
}
