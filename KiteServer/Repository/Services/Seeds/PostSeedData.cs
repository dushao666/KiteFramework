using Domain.System;
using Microsoft.Extensions.Configuration;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class PostSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly IConfiguration _configuration;

        public PostSeedData(ISugarUnitOfWork<DbContext> unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查是否需要重新创建数据库
                bool recreateDatabase = _configuration.GetValue<bool>("SeedData:RecreateDatabase", false);
                
                // 如果不是重新创建数据库，则检查表中是否已有数据
                if (!recreateDatabase)
                {
                    // 检查岗位表是否有数据
                    var postCount = context.Posts.AsQueryable().Count();
                    if (postCount > 0)
                    {
                        Console.WriteLine("岗位数据已存在，跳过初始化");
                        return;
                    }
                }

                try
                {
                    Console.WriteLine("开始初始化岗位数据...");
                    
                    // 创建默认岗位
                    var defaultPosts = new List<Post>
                    {
                        new Post
                        {
                            Code = "CEO",
                            Name = "董事长",
                            Sort = 1,
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Remark = "董事长岗位"
                        },
                        new Post
                        {
                            Code = "SE",
                            Name = "项目经理",
                            Sort = 2,
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Remark = "项目经理岗位"
                        },
                        new Post
                        {
                            Code = "HR",
                            Name = "人力资源",
                            Sort = 3,
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Remark = "人力资源岗位"
                        },
                        new Post
                        {
                            Code = "USER",
                            Name = "普通员工",
                            Sort = 4,
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Remark = "普通员工岗位"
                        }
                    };

                    // 插入岗位数据
                    context.Posts.InsertRange(defaultPosts);
                    context.Commit();
                    
                    Console.WriteLine("岗位数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"岗位数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 