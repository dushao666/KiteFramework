import request from '../utils/request';

// 菜单项接口
export interface MenuItem {
  id: number;
  name: string;
  path: string;
  icon?: string;
  parentId?: number;
  children?: MenuItem[];
}

// API响应接口
export interface ApiResponse<T> {
  code: number;
  message: string;
  data: T;
}

// 获取菜单树
export async function getMenuTree(): Promise<ApiResponse<MenuItem[]>> {
  // 这里是模拟的API调用，实际项目中应该调用真实的后端接口
  return {
    code: 200,
    message: 'success',
    data: [
      {
        id: 1,
        name: '首页',
        path: '/home',
        icon: 'HomeFilled',
        children: []
      },
      {
        id: 2,
        name: '导入明细',
        path: '/import',
        icon: 'Document',
        children: []
      }
    ]
  };
}

// 获取菜单列表
export function getMenuList(params?: any): Promise<ApiResponse<MenuItem[]>> {
  return request({
    url: '/system/menu/list',
    method: 'GET',
    params
  });
}

// 添加菜单
export function addMenu(data: MenuItem): Promise<ApiResponse<any>> {
  return request({
    url: '/system/menu',
    method: 'POST',
    data
  });
}

// 更新菜单
export function updateMenu(data: MenuItem): Promise<ApiResponse<any>> {
  return request({
    url: `/system/menu/${data.id}`,
    method: 'PUT',
    data
  });
}

// 删除菜单
export function deleteMenu(id: number): Promise<ApiResponse<any>> {
  return request({
    url: `/system/menu/${id}`,
    method: 'DELETE'
  });
}
