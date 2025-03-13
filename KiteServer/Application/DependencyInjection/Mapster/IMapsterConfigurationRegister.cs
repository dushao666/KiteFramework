namespace Application.DependencyInjection.Mapster
{
    /// <summary>
    /// Mapster 配置注册接口
    /// </summary>
    public interface IMapsterConfigurationRegister
    {
        void Register(TypeAdapterConfig config);
    }
} 