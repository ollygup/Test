using System.Text;

namespace Test.Application.Helper
{
    public static class DecodingHelper
    {
        public static string DecodeFromBase64(string base64Encoded)
        {
            byte[] bytes = Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
