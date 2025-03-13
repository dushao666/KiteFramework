using Domain.System;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class PostSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;

        public PostSeedData(ISugarUnitOfWork<DBContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查岗位表是否有数据
                var postCount = context.Posts.AsQueryable().Count();
                if (postCount > 0)
                {
                    return;
                }

                try
                {
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
                    
                    Console.WriteLine("岗位种子数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"岗位种子数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 