using System.Security.Cryptography;
using System.Text;

namespace Test.Application.Helper
{
    public static class HashHelper
    {
        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder hexString = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    hexString.AppendFormat("{0:x2}", b); // to lower case
                }

                return hexString.ToString();
            }
        }
    }
}
