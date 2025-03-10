import request from '../utils/request'
import { ApiResponse, LoginResponseData } from '../types/api'

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