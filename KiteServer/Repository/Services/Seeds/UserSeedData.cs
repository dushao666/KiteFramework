 using Domain.System;
using Domain.UserInfo;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class UserSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

        public UserSeedData(ISugarUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // ����û����Ƿ�������
                var userCount = context.Users.AsQueryable().Count();
                if (userCount > 0)
                {
                    return;
                }

                try
                {
                    // ����Ĭ�Ϲ���Ա�û�
                    var adminUser = new UserInfo
                    {
                        Name = "admin",
                        NickName = "admin",
                        PassWord = "123456", // ʵ��Ӧ����Ӧ�ü��ܴ洢
                        DingUserId = "666666",
                        Status = "0",
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    // �����û�����
                    context.Users.Insert(adminUser);
                    context.Commit();
                    
                    // ��ȡ����Ա��ɫ
                    var adminRole = context.Roles.GetFirst(r => r.Code == "admin");
                    if (adminRole != null)
                    {
                        // Ϊ����Ա�û��������Ա��ɫ
                        var userRole = new UserRole
                        {
                            UserId = adminUser.Id,
                            RoleId = adminRole.Id,
                            CreateBy = "system",
                            UpdateBy = "system"
                        };
                        
                        // ����û���ɫ����
                        context.UserRoles.Insert(userRole);
                        context.Commit();
                    }
                    
                    Console.WriteLine("�û��������ݳ�ʼ���ɹ�");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"�û��������ݳ�ʼ��ʧ��: {ex.Message}");
                    throw;
                }
            }
        }
    }
}