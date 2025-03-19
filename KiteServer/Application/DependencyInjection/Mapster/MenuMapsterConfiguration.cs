using Domain.System;
using DomainShared.Dto.System;
using Application.Commands.System.Menu;
using System.Text.Json;

namespace Application.DependencyInjection.Mapster
{
    public class MenuMapsterConfiguration : IMapsterConfigurationRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 定义 JSON 序列化选项
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 不转义中文字符
                WriteIndented = false
            };

            // 菜单相关的映射配置
            config.NewConfig<Menu, MenuDto>()
                .Map(dest => dest.Meta, src => src.Meta);

            // CreateMenuCommand 到 Menu 的映射
            config.NewConfig<CreateMenuCommand, Menu>()
                .Map(dest => dest.Meta, src => src.Meta)
                .Map(dest => dest.MetaJson, src => src.Meta != null ? JsonSerializer.Serialize(src.Meta, jsonOptions) : null);

            // UpdateMenuCommand 到 Menu 的映射
            config.NewConfig<UpdateMenuCommand, Menu>()
                .Map(dest => dest.Meta, src => src.Meta)
                .Map(dest => dest.MetaJson, src => src.Meta != null ? JsonSerializer.Serialize(src.Meta, jsonOptions) : null);
        }
    }
} 