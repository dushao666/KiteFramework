namespace Infrastructure.Extension;

public static class StringExtensions
{
    /// <summary>
    /// 移除末尾指定字符串
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <param name="postFixes">要移除的字符串</param>
    public static string RemovePostFix(this string str, params string[] postFixes)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        if (postFixes.Count() == 0) return str;
        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix)) return str.Substring(str.Length - postFix.Length);
        }
        return str;
    }
    public static bool IsSsmlContent(this string content)
    {
        return content.Trim().StartsWith("<speak") && content.Trim().EndsWith("</speak>");
    }
}