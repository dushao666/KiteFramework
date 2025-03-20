import request from '../utils/request'
import { ApiResponse, LoginResponseData } from '../types/api'
import { ref } from 'vue'

export interface LoginParams {
  Login: string
  Password: string
  Type: number
  DingTalkAuthCode: string
}

// 用户相关接口
export function login(data: LoginParams): Promise<ApiResponse<LoginResponseData>> {
  return request({
    url: '/login/signIn',
    method: 'POST',
    data
  })
}

export function logout() {
  return request({
    url: '/login/signOut',
    method: 'POST'
  })
}

export interface UserItem {
  id: string | number;
  name: string;
  nickName: string;
  status: string | number;
  dingUserId?: string;
  roles?: string[];
  roleIds?: number[];
  postIds?: number[];
  createTime?: string;
  updateTime?: string;
}

export interface UserQuery {
  name?: string;
  status?: string;
  pageNum?: number;
  pageSize?: number;
}

export interface UserPassword {
  userId: string | number;
  oldPassword?: string;
  newPassword: string;
}

export interface UserRole {
  userId: number;
  roleIds: number[];
}

export interface UserPost {
  userId: number;
  postIds: number[];
}

// 导入编辑状态标记
export const isEditingUser = ref(false)

// 获取用户列表
export function getUserList(params = {}): Promise<ApiResponse<UserItem[]>> {
  return request({
    url: '/user/list',
    method: 'GET',
    params
  });
}

// 获取用户详情
export function getUserDetail(id: string | number): Promise<ApiResponse<UserItem>> {
  return request({
    url: `/user/${id}`,
    method: 'GET'
  });
}

// 添加用户
export function addUser(data: UserItem): Promise<ApiResponse<any>> {
  return request({
    url: '/user',
    method: 'POST',
    data
  });
}

// 更新用户
export function updateUser(data: UserItem): Promise<ApiResponse<any>> {
  return request({
    url: `/user/${data.id}`,
    method: 'PUT',
    data
  });
}

// 删除用户
export function deleteUser(id: string | number): Promise<ApiResponse<any>> {
  return request({
    url: `/user/${id}`,
    method: 'DELETE'
  });
}

// 更新用户状态
export function updateUserStatus(id: string | number, status: string | number): Promise<ApiResponse<any>> {
  // 确保status是数字类型
  const statusVal = typeof status === 'string' ? parseInt(status) : status;
  return request({
    url: `/user/status`,
    method: 'PUT',
    data: { id, status: statusVal }
  });
}

// 重置用户密码
export function resetUserPassword(data: UserPassword): Promise<ApiResponse<any>> {
  return request({
    url: `/user/password/reset`,
    method: 'PUT',
    data
  });
}

// 获取用户角色
export function getUserRoles(userId: string | number): Promise<ApiResponse<number[]>> {
  // 检查是否在编辑用户状态
  if (isEditingUser.value) {
    console.warn('在编辑用户过程中调用了getUserRoles API，这可能是不必要的');
  }
  
  return request({
    url: `/user/roles/${userId}`,
    method: 'GET'
  });
}

// 分配用户角色
export function assignUserRoles(userId: string | number, roleIds: number[]): Promise<ApiResponse<any>> {
  return request({
    url: `/user/roles/${userId}`,
    method: 'PUT',
    data: { roleIds }
  });
}

// 获取用户岗位
export function getUserPosts(userId: string | number): Promise<ApiResponse<number[]>> {
  // 检查是否在编辑用户状态
  if (isEditingUser.value) {
    console.warn('在编辑用户过程中调用了getUserPosts API，这可能是不必要的');
  }
  
  return request({
    url: `/user/posts/${userId}`,
    method: 'GET'
  });
}

// 分配用户岗位
export function assignUserPosts(userId: string | number, postIds: number[]): Promise<ApiResponse<any>> {
  return request({
    url: `/user/posts/${userId}`,
    method: 'PUT',
    data: { postIds }
  });
} 