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
  // 这里是模拟的API调用，实际项目中应该调用真实的后端接口
  return {
    code: 200,
    message: 'success',
    data: [
      {
        id: 1,
        name: '首页',
        path: '/home',
        icon: 'HomeFilled'
      },
      {
        id: 2,
        name: '系统管理',
        path: '/system',
        icon: 'Setting',
        children: [
          {
            id: 21,
            name: '用户管理',
            path: '/system/user',
            icon: 'User',
            parentId: 2
          },
          {
            id: 22,
            name: '角色管理',
            path: '/system/role',
            icon: 'UserFilled',
            parentId: 2
          },
          {
            id: 23,
            name: '菜单管理',
            path: '/system/menu',
            icon: 'Menu',
            parentId: 2
          },
          {
            id: 24,
            name: '部门管理',
            path: '/system/dept',
            icon: 'Office',
            parentId: 2
          }
        ]
      },
      {
        id: 3,
        name: '监控管理',
        path: '/monitor',
        icon: 'Monitor',
        children: [
          {
            id: 31,
            name: '在线用户',
            path: '/monitor/online',
            icon: 'Connection',
            parentId: 3
          },
          {
            id: 32,
            name: '操作日志',
            path: '/monitor/log',
            icon: 'Document',
            parentId: 3
          },
          {
            id: 33,
            name: '系统性能',
            path: '/monitor/server',
            icon: 'Cpu',
            parentId: 3
          }
        ]
      },
      {
        id: 4,
        name: '系统工具',
        path: '/tool',
        icon: 'Tools',
        children: [
          {
            id: 41,
            name: '代码生成',
            path: '/tool/gen',
            icon: 'Edit',
            parentId: 4
          },
          {
            id: 42,
            name: '系统接口',
            path: '/tool/swagger',
            icon: 'Link',
            parentId: 4
          }
        ]
      }
    ]
  };
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
