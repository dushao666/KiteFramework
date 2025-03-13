using Application.Queries.System.Role;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 获取角色菜单查询处理器
/// </summary>
public class GetRoleMenusQueryHandler : IRequestHandler<GetRoleMenusQuery, List<long>>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

    public GetRoleMenusQueryHandler(ISugarUnitOfWork<DbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<long>> Handle(GetRoleMenusQuery request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Roles.Context;
            
            // 检查角色是否存在
            var role = await context.Roles.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            // 获取角色关联的菜单ID列表
            var menuIds = await db.Queryable<Domain.System.RoleMenu>()
                .Where(rm => rm.RoleId == request.RoleId && !rm.IsDeleted)
                .Select(rm => rm.MenuId)
                .ToListAsync();

            return menuIds;
        }
    }
} 