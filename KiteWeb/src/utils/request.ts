import axios, { AxiosError } from 'axios'
import { useUserStore } from '../stores/userStore'
import { ElMessage } from 'element-plus'
import router from '../router'
import { ApiErrorResponse } from '../types/api'

// 创建axios实例
const service = axios.create({
  baseURL: import.meta.env.VITE_APP_BASE_API,
  timeout: 600000,  // 10分钟
  maxBodyLength: Infinity,
  maxContentLength: Infinity,
  headers: {
    'Content-type': 'application/json;charset=utf-8',
  },
})

// 请求拦截器
service.interceptors.request.use(
  config => {
    // 处理请求头中的Content-Type
    if (config.data instanceof FormData) {
      config.headers['Content-Type'] = 'multipart/form-data'
    }

    const { url } = config
    // 不需要token的白名单
    const whiteList = ['/login', '/register']
    if (!whiteList.some(path => url?.includes(path))) {
      const userStore = useUserStore()
      const token = userStore.jwtAccessToken
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      } else {
        // token不存在时跳转登录
        router.push('/login')
        return Promise.reject(new Error('请先登录'))
      }
    }
    return config
  },
  error => {
    console.error('请求错误:', error)
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  response => {
    const res = response.data
    if (res.code !== 200) {
      ElMessage.error(res.message || '操作失败')
      return Promise.reject(new Error(res.message || '操作失败'))
    }
    return res
  },
  async (error: AxiosError<ApiErrorResponse>) => {
    console.error('响应错误:', error)
    const userStore = useUserStore()

    if (error.code === 'ECONNABORTED') {
      ElMessage.error('请求超时，请重试')
    } else if (error.response) {
      const errorData = error.response.data
      switch (error.response.status) {
        case 401:
          ElMessage.error('登录已过期，请重新登录')
          userStore.logout()
          await router.push('/login')
          break
        case 403:
          ElMessage.error('没有权限访问')
          break
        case 404:
          ElMessage.error('请求的资源不存在')
          break
        case 400:
          ElMessage.error(errorData?.message || '请求参数错误')
          break
        case 500:
          ElMessage.error(errorData?.message || '服务器内部错误')
          break
        default:
          ElMessage.error('未知错误，请联系管理员')
      }
    } else if (error.request) {
      ElMessage.error('网络错误，请检查网络连接')
    } else {
      ElMessage.error('请求配置错误')
    }

    return Promise.reject(error)
  }
)

export default service 