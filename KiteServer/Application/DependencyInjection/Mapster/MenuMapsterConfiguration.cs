using Domain.System;
using DomainShared.Dto.System;

namespace Application.DependencyInjection.Mapster
{
    public class MenuMapsterConfiguration : IMapsterConfigurationRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 菜单相关的映射配置
            config.NewConfig<Menu, MenuDto>();
            // ... 其他菜单相关映射
        }
    }
} 