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
  component?: string; // 组件路径，如 'views/system/menu/index'
  meta?: {
    title?: string;    // 页面标题
    requiresAuth?: boolean; // 是否需要认证
    keepAlive?: boolean;    // 是否缓存组件
    icon?: string;          // 图标
    roles?: string[];       // 允许访问的角色
    [key: string]: any;     // 其他元数据
  };
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
    url: '/menu/menuTree',
    method: 'GET'
  });
}

// 获取当前用户的菜单
export async function getUserMenus(): Promise<ApiResponse<MenuItem[]>> {
  return request({
    url: '/menu/userMenus',
    method: 'GET'
  });
}

// 获取菜单列表
export function getMenuList(params = {}): Promise<ApiResponse<MenuItem[]>> {
  // 确保 params 是一个对象
  const safeParams = params || {};
  
  return request({
    url: '/menu/menuList',
    method: 'GET',
    params: safeParams
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
