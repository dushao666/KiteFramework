using Domain.System;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class RoleSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

        public RoleSeedData(ISugarUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // ����ɫ���Ƿ�������
                var roleCount = context.Roles.AsQueryable().Count();
                if (roleCount > 0)
                {
                    return;
                }

                try
                {
                    // ����Ĭ�Ͻ�ɫ
                    var adminRole = new Role
                    {
                        Name = "��������Ա",
                        Code = "admin",
                        Description = "ϵͳ��������Ա��ӵ������Ȩ��",
                        Status = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    var operatorRole = new Role
                    {
                        Name = "��ͨ�û�",
                        Code = "user",
                        Description = "��ͨ�û���ӵ�л���Ȩ��",
                        Status = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    // �����ɫ����
                    context.Roles.Insert(adminRole);
                    context.Roles.Insert(operatorRole);
                    context.Commit();
                    
                    // ��ȡ���в˵�ID
                    var menuIds = context.Menus.AsQueryable()
                        .Where(m => !m.IsDeleted)
                        .Select(m => m.Id)
                        .ToList();
                    
                    // Ϊ����Ա��ɫ�������в˵�Ȩ��
                    if (menuIds.Any())
                    {
                        var adminRoleMenus = menuIds.Select(menuId => new RoleMenu
                        {
                            RoleId = adminRole.Id,
                            MenuId = menuId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }).ToList();
                        
                        context.RoleMenus.InsertRange(adminRoleMenus);
                        context.Commit();
                    }
                    
                    Console.WriteLine("��ɫ�������ݳ�ʼ���ɹ�");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"��ɫ�������ݳ�ʼ��ʧ��: {ex.Message}");
                    throw;
                }
            }
        }
    }
}