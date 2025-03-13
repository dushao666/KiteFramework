using Application.Queries.System.Role;
using DomainShared.Dto.System;
using Infrastructure.Exceptions;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 获取角色详情查询处理器
/// </summary>
public class GetRoleDetailQueryHandler : IRequestHandler<GetRoleDetailQuery, RoleDto>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly IMapper _mapper;

    public GetRoleDetailQueryHandler(ISugarUnitOfWork<DbContext> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RoleDto> Handle(GetRoleDetailQuery request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var role = await context.Roles.GetByIdAsync(request.Id);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            return _mapper.Map<RoleDto>(role);
        }
    }
} 