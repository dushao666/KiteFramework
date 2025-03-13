using Application.DependencyInjection.Mapster;

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
        // 自动获取所有映射配置
        var registers = typeof(MapsterConfiguration).Assembly
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IMapsterConfigurationRegister).IsAssignableFrom(t))
            .Select(t => (IMapsterConfigurationRegister)Activator.CreateInstance(t))
            .ToList();

        // 注册所有映射配置
        registers.ForEach(register => register.Register(config));
        
    }
}
