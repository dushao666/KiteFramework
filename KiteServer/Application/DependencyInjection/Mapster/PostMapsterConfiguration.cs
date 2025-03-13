using Domain.System;
using DomainShared.Dto.System;
using Application.Commands.System.Post;

namespace Application.DependencyInjection.Mapster
{
    public class PostMapsterConfiguration : IMapsterConfigurationRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 配置 Post 到 PostDto 的映射
            config.NewConfig<Post, PostDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Sort, src => src.Sort)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.Remark, src => src.Remark)
                .Map(dest => dest.CreateTime, src => src.CreateTime)
                .Map(dest => dest.UpdateTime, src => src.UpdateTime);

            // 配置 CreatePostCommand 到 Post 的映射
            config.NewConfig<CreatePostCommand, Post>()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Sort, src => src.Sort)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.Remark, src => src.Remark);
        }
    }
} 