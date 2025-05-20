using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoMedico.Utils.Encrypt
{
    public class EncryptUtils
    {
        private const string EncryptionKey = "@AlWaLç_";
        private static readonly byte[] Salt =
            { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
          0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public static string EncryptPasswordBase64(string password)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string DecryptPasswordBase64(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static string EncryptPassword(string password)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);
            using (Aes encrypt = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);

                encrypt.Key = pdb.GetBytes(32);
                encrypt.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encrypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }

            return password;
        }

        public static string DecryptPassword(string password)
        {
            byte[] passBytes = Convert.FromBase64String(password);
            using (Aes encrypt = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                encrypt.Key = pdb.GetBytes(32);
                encrypt.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encrypt.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(passBytes, 0, passBytes.Length);
                        cs.Close();
                    }
                    password = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return password;
        }
    }
}
