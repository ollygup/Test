using System.Text;

namespace Test.Application.Helper
{
    public static class EncodingHelper
    {
        public static string EncodeToBase64(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(byteArray);
        }
    }
}
