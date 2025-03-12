import request from '../utils/request';

// 菜单项接口
export interface MenuItem {
  id: number;
  name: string;
  path: string;
  icon?: string;
  parentId?: number;
  children?: MenuItem[];
  sort?: number;
  isHidden?: boolean;
}

// API响应接口
export interface ApiResponse<T> {
  code: number;
  message: string;
  data: T;
}

// 获取菜单树
export async function getMenuTree(): Promise<ApiResponse<MenuItem[]>> {
  return request({
    url: '/menu/tree',
    method: 'GET'
  });
}

// 获取菜单列表
export function getMenuList(params?: any): Promise<ApiResponse<MenuItem[]>> {
  return request({
    url: '/menu/list',
    method: 'GET',
    params
  });
}

// 添加菜单
export function addMenu(data: MenuItem): Promise<ApiResponse<any>> {
  return request({
    url: '/menu/addMenu',
    method: 'POST',
    data
  });
}

// 更新菜单
export function updateMenu(data: MenuItem): Promise<ApiResponse<any>> {
  return request({
    url: `/menu/${data.id}`,
    method: 'PUT',
    data
  });
}

// 删除菜单
export function deleteMenu(id: number): Promise<ApiResponse<any>> {
  return request({
    url: `/menu/${id}`,
    method: 'DELETE'
  });
}
