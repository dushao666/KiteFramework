using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utility;

public static class EncryptionHelper
{
    /// <summary>
    /// AES加密字符串
    /// </summary>
    public static string EncryptBytes(string data, string secretKey)
    {
        byte[] secretByts = Encoding.UTF8.GetBytes(secretKey);
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        using (Aes aes = Aes.Create())
        {
            aes.Key = secretByts;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform decryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] decValue = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Convert.ToBase64String(decValue);
        }
    }
    /// <summary>
    /// AES解密字符串
    /// </summary>
    public static string DecryptBytes(string data, string secretKey)
    {
        byte[] secretByts = Encoding.UTF8.GetBytes(secretKey);
        byte[] buffer = Convert.FromBase64String(data);
        using (Aes aes = Aes.Create())
        {
            aes.Key = secretByts;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] decValue = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(decValue);
        }
    }

    /// <summary>
    /// 将输入的字符串使用SHA512方式进行哈希散列
    /// </summary>
    public static string Sha512(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        using (var sha = SHA512.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
