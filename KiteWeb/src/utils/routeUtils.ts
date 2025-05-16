import { useUserStore } from '../stores/userStore'
import { useRouteStore } from '../stores/routeStore'
import { loadUserMenus, resetRouter } from '../router'
import { ElMessage } from 'element-plus'
import router from '../router'
import Layout from '../components/layout/index.vue'
import { RouteRecordRaw } from 'vue-router'

/**
 * 初始化路由
 * 当页面刷新时，可以调用此方法来恢复路由状态
 */
export const initRoutes = async (): Promise<boolean> => {
  const userStore = useUserStore()
  const routeStore = useRouteStore()
  const isLoggedIn = userStore.isLoggedIn
  const token = userStore.jwtAccessToken
  
  // 如果没有登录，则不需要加载路由
  if (!isLoggedIn || !token) {
    return false
  }
  
  // 检查路由状态
  const routesAdded = routeStore.getDynamicRoutesAdded
  const savedRoutes = routeStore.getAllRoutes
  
  // 如果没有路由状态或路由状态过期，重新加载
  if (!routesAdded || routeStore.isMenuExpired) {
    try {
      // 重置路由，确保干净状态
      await resetRouter()
      
      // 重新加载用户菜单和路由
      const success = await loadUserMenus()
      if (!success) {
        return false
      }
      
      return true
    } catch (error) {
      return false
    }
  }
  
  return true
}

/**
 * 从路径自动生成可能的视图组件路径
 * @param path 路由路径
 * @returns 可能的视图组件路径数组
 */
export const getPossibleViewPaths = (path: string): string[] => {
  // 移除开头的 / 和结尾的 /
  const cleanPath = path.replace(/^\/|\/$/g, '')
  
  // 拆分路径
  const segments = cleanPath.split('/')
  
  // 可能的组件路径
  const possiblePaths: string[] = []
  
  // 1. /xxx/yyy 格式 -> xxx/yyy/index.vue
  possiblePaths.push(`${cleanPath}/index.vue`)
  
  // 2. /xxx/yyy 格式 -> xxx/yyy.vue
  possiblePaths.push(`${cleanPath}.vue`)
  
  // 3. 如果是多级路径，尝试 /xxx/yyy -> xxx/yyy/index.vue
  if (segments.length >= 2) {
    possiblePaths.push(`${segments.join('/')}/index.vue`)
  }
  
  // 4. 如果是二级路径，尝试 /xxx/yyy -> xxx/yyy.vue
  if (segments.length === 2) {
    possiblePaths.push(`${segments[0]}/${segments[1]}.vue`)
  }
  
  return possiblePaths
}

/**
 * 动态创建路由
 * @param path 路由路径
 * @returns 路由对象
 */
export const createDynamicRoute = (path: string): RouteRecordRaw => {
  // 移除开头的 / 并拆分路径
  const cleanPath = path.replace(/^\//, '')
  const segments = cleanPath.split('/')
  
  // 一级路径
  const topLevelPath = '/' + segments[0]
  
  // 子路径
  const remainingPath = segments.slice(1).join('/')
  
  // 创建子路由
  const childRoute: RouteRecordRaw = {
    path: remainingPath,
    name: remainingPath.replace(/\//g, '-'),
    component: () => {
      // 尝试加载组件
      const viewPath = cleanPath.replace(/\/\w+$/, '/index')
      
      return import(`../views/${viewPath}.vue`)
        .catch(error => {
          // 返回404组件
          return import('../views/error/404.vue')
        })
    },
    meta: {
      title: segments[segments.length - 1],
      requiresAuth: true
    }
  }
  
  // 创建顶级路由
  const route: RouteRecordRaw = {
    path: topLevelPath,
    name: segments[0],
    component: Layout,
    children: [childRoute]
  }
  
  return route
} 