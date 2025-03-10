using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utility;

public static class CommonHelper
{
    public static string GenerateRandom(int? codeCount = 8)
    {
        if (!codeCount.HasValue)
        {
            codeCount = 8;
        }

        int num = 0;
        string text = string.Empty;
        long num2 = DateTime.Now.Ticks + num;
        num++;
        Random random = new Random((int)(num2 & 0xFFFFFFFFu) | (int)(num2 >> num));
        for (int i = 0; i < codeCount; i++)
        {
            int num3 = random.Next();
            text += ((num3 % 2 != 0) ? ((char)(65 + (ushort)(num3 % 26))) : ((char)(48 + (ushort)(num3 % 10))));
        }

        return text;
    }

    public static string GenerateRandomNumber(int codeCount = 6)
    {
        string text = "123456789";
        string text2 = string.Empty;
        Random random = new Random();
        while (text2.Length < codeCount)
        {
            string text3 = text[random.Next(0, text.Length)].ToString();
            if (text2.IndexOf(text3) == -1)
            {
                text2 += text3;
            }
        }

        return text2;
    }

    public static long ProbabilityRandomRumber(List<Tuple<long, double?>> items)
    {
        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        byte[] array = new byte[4];
        randomNumberGenerator.GetBytes(array);
        Random random = new Random(BitConverter.ToInt32(array, 0));
        if (items.Sum((Tuple<long, double?> t) => t.Item2) > 1.0)
        {
            throw new Exception("随机因子总和不能超过1");
        }

        if (items.Any((Tuple<long, double?> t) => t.Item2 <= 0.0))
        {
            int index = random.Next(1, items.Count);
            return items[index].Item1;
        }

        double? num = 0.0;
        double num2 = random.NextDouble();
        int index2 = 0;
        for (int i = 0; i < items.Count; i++)
        {
            num += items[i].Item2;
            if (num2 < num)
            {
                index2 = i;
                break;
            }
        }

        return items[index2].Item1;
    }

    public static long ProbabilityRandomRumber(List<Tuple<long>> items)
    {
        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        byte[] array = new byte[4];
        randomNumberGenerator.GetBytes(array);
        Random random = new Random(BitConverter.ToInt32(array, 0));
        int index = random.Next(1, items.Count);
        return items[index].Item1;
    }

    public static string HideSensitiveInfo(this string info, int left, int right, bool basedOnLeft = true)
    {
        if (string.IsNullOrEmpty(info))
        {
            return "";
        }

        StringBuilder stringBuilder = new StringBuilder();
        int num = info.Length - left - right;
        if (num > 0)
        {
            string value = info.Substring(0, left);
            string value2 = info.Substring(info.Length - right);
            stringBuilder.Append(value);
            for (int i = 0; i < num; i++)
            {
                stringBuilder.Append("*");
            }

            stringBuilder.Append(value2);
        }
        else if (basedOnLeft)
        {
            if (info.Length > left && left > 0)
            {
                stringBuilder.Append(info.Substring(0, left) + "****");
            }
            else
            {
                stringBuilder.Append(info.Substring(0, 1) + "****");
            }
        }
        else if (info.Length > right && right > 0)
        {
            stringBuilder.Append("****" + info.Substring(info.Length - right));
        }
        else
        {
            stringBuilder.Append("****" + info.Substring(info.Length - 1));
        }

        return stringBuilder.ToString();
    }
}
