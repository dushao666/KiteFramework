using Application.Commands.System.Role;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 添加角色命令处理器
/// </summary>
public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, long>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public AddRoleCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, IMapper mapper, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<long> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Roles.Context;
            
            // 检查角色编码是否已存在
            var existingRole = await db.Queryable<Domain.System.Role>()
                .FirstAsync(r => r.Code == request.Code && !r.IsDeleted);
                
            if (existingRole != null)
            {
                throw new UserFriendlyException("角色编码已存在");
            }

            // 创建角色
            var role = _mapper.Map<Domain.System.Role>(request);
            role.CreateBy = _currentUser.LoginName ?? "system";
            role.UpdateBy = _currentUser.LoginName ?? "system";

            // 保存角色
            await context.Roles.InsertAsync(role);
            context.Commit();

            return role.Id;
        }
    }
} 