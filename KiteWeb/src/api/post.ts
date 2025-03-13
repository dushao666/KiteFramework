import request from '../utils/request';

// 岗位项接口
export interface PostItem {
  id: number;
  code: string;
  name: string;
  sort: number;
  status: number;
  remark?: string;
  createTime?: string;
  updateTime?: string;
}

// API响应接口
export interface ApiResponse<T> {
  code: number;
  message: string;
  data: T;
}

// 获取岗位列表
export function getPostList(params = {}): Promise<ApiResponse<PostItem[]>> {
  return request({
    url: '/post/list',
    method: 'GET',
    params
  });
}

// 获取岗位详情
export function getPostDetail(id: number): Promise<ApiResponse<PostItem>> {
  return request({
    url: `/post/${id}`,
    method: 'GET'
  });
}

// 添加岗位
export function addPost(data: PostItem): Promise<ApiResponse<any>> {
  return request({
    url: '/post',
    method: 'POST',
    data
  });
}

// 更新岗位
export function updatePost(data: PostItem): Promise<ApiResponse<any>> {
  return request({
    url: `/post/${data.id}`,
    method: 'PUT',
    data
  });
}

// 删除岗位
export function deletePost(id: number): Promise<ApiResponse<any>> {
  return request({
    url: `/post/${id}`,
    method: 'DELETE'
  });
}

// 更新岗位状态
export function updatePostStatus(id: number, status: number): Promise<ApiResponse<any>> {
  return request({
    url: `/post/status`,
    method: 'PUT',
    data: { id, status }
  });
}