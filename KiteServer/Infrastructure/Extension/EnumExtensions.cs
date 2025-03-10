namespace Infrastructure.Extension;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    public static int GetValue(this Enum value)
    {
        return Convert.ToInt32(value);
    }
}
