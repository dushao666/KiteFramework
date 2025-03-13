import request from '../utils/request'

export interface RoleItem {
  id?: number;
  name: string;
  code: string;
  description?: string;
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

// 获取角色列表
export function getRoleList(params: RoleQuery) {
  return request({
    url: '/role/list',
    method: 'get',
    params
  })
}

// 获取角色详情
export function getRoleDetail(id: number) {
  return request({
    url: `/role/${id}`,
    method: 'get'
  })
}

// 添加角色
export function addRole(data: RoleItem) {
  return request({
    url: '/role',
    method: 'post',
    data
  })
}

// 更新角色
export function updateRole(data: RoleItem) {
  return request({
    url: '/role',
    method: 'put',
    data
  })
}

// 删除角色
export function deleteRole(id: number) {
  return request({
    url: `/role/${id}`,
    method: 'delete'
  })
}

// 更新角色状态
export function updateRoleStatus(id: number, status: number) {
  return request({
    url: `/role/status`,
    method: 'put',
    data: { id, status }
  })
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