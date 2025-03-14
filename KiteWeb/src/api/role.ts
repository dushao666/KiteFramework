import request from '../utils/request'

export interface RoleItem {
  id: string | number;
  name: string;
  code: string;
  description: string;
  status: number;
  createTime?: string;
  updateTime?: string;
}

export interface RoleQuery {
  keyword?: string;
  status?: number;
  pageNum?: number;
  pageSize?: number;
}

export interface RolePermission {
  roleId: number;
  menuIds: number[];
}

// API响应接口
export interface ApiResponse<T> {
  code: number;
  message: string;
  data: T;
  total?: number;
}

// 获取角色列表
export function getRoleList(params = {}): Promise<ApiResponse<RoleItem[]>> {
  return request({
    url: '/role/list',
    method: 'GET',
    params
  });
}

// 获取角色详情
export function getRoleDetail(id: string | number): Promise<ApiResponse<RoleItem>> {
  return request({
    url: `/role/${id}`,
    method: 'GET'
  });
}

// 添加角色
export function addRole(data: RoleItem): Promise<ApiResponse<any>> {
  return request({
    url: '/role',
    method: 'POST',
    data
  });
}

// 更新角色
export function updateRole(data: RoleItem): Promise<ApiResponse<any>> {
  return request({
    url: `/role/${data.id}`,
    method: 'PUT',
    data
  });
}

// 删除角色
export function deleteRole(id: string | number): Promise<ApiResponse<any>> {
  return request({
    url: `/role/${id}`,
    method: 'DELETE'
  });
}

// 更新角色状态
export function updateRoleStatus(id: string | number, status: number): Promise<ApiResponse<any>> {
  return request({
    url: `/role/status`,
    method: 'PUT',
    data: { id, status }
  });
}

// 获取角色权限
export function getRolePermissions(roleId: string | number): Promise<ApiResponse<number[]>> {
  return request({
    url: `/role/permissions/${roleId}`,
    method: 'GET'
  });
}

// 保存角色权限
export function saveRolePermissions(roleId: string | number, menuIds: number[]): Promise<ApiResponse<any>> {
  return request({
    url: `/role/permissions/${roleId}`,
    method: 'PUT',
    data: { menuIds }
  });
}

// 获取角色的权限菜单
export function getRoleMenus(roleId: number) {
  return request({
    url: `/role/menus/${roleId}`,
    method: 'get'
  })
}

// 分配角色权限
export function assignRoleMenus(data: RolePermission) {
  return request({
    url: '/role/menus',
    method: 'put',
    data
  })
} 