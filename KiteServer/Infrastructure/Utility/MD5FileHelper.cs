using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utility;

public static class MD5FileHelper
{
    public static string CalculateMD5FromFile(string filePath)
    {
        using (MD5 md5 = MD5.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
    public static string CalculateMD5FromString(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
