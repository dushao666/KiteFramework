namespace Application.DependencyInjection;

/// <summary>
/// Mapster配置
/// </summary>
public class MapsterConfiguration : IRegister
{
    /// <summary>
    /// 注册
    /// </summary>
    public void Register(TypeAdapterConfig config)
    {
        // config.ForType<VoiceFile, VoiceFilePageListDto>().Map(dest => dest.Size, source => (source.Size / 1024).ToString("F2"));
    }
}
