using Application.Queries.System.Role;
using DomainShared.Dto;
using DomainShared.Dto.System;
using Infrastructure.Utility;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 获取角色列表查询处理器
/// </summary>
public class GetRoleListQueryHandler : IRequestHandler<GetRoleListQuery, PagedList<RoleDto>>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly IMapper _mapper;

    public GetRoleListQueryHandler(ISugarUnitOfWork<DbContext> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<RoleDto>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Roles.Context;
            var query = db.Queryable<Domain.System.Role>().Where(r => !r.IsDeleted);

            // 名称搜索
            if (!string.IsNullOrEmpty(request.QueryDto.Name))
            {
                query = query.Where(r => r.Name.Contains(request.QueryDto.Name));
            }

            // 编码搜索
            if (!string.IsNullOrEmpty(request.QueryDto.Code))
            {
                query = query.Where(r => r.Code.Contains(request.QueryDto.Code));
            }

            // 描述搜索
            if (!string.IsNullOrEmpty(request.QueryDto.Description))
            {
                query = query.Where(r => r.Description.Contains(request.QueryDto.Description));
            }

            // 状态筛选
            if (request.QueryDto.Status.HasValue)
            {
                query = query.Where(r => r.Status == request.QueryDto.Status.Value);
            }

            // 获取总数
            var total = await query.CountAsync();

            // 分页查询
            var roles = await query
                .OrderByDescending(r => r.CreateTime)
                .Skip((request.QueryDto.PageNum - 1) * request.QueryDto.PageSize)
                .Take(request.QueryDto.PageSize)
                .ToListAsync();

            // 映射为DTO
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);

            return new PagedList<RoleDto>(total, roleDtos);
        }
    }
} 