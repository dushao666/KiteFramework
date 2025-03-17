import { RouteRecordRaw } from 'vue-router'
import { MenuItem } from '../api/menu'
import Layout from '../components/layout/index.vue'
import { ElMessage } from 'element-plus'

// 组件映射表，用于快速查找组件
const componentMap: Record<string, any> = {
  Layout: Layout,
}

/**
 * 根据路径推断组件路径
 * @param path 菜单路径
 * @returns 推断的组件路径
 */
export const inferComponentPath = (path: string): string => {
  // 如果是根路径，返回Layout
  if (path === '/' || path === '') {
    return 'Layout'
  }
  
  // 移除开头的 /
  let componentPath = path.replace(/^\//, '')
  
  // 如果路径中没有子路径（如 /home），则返回 home/index
  if (!componentPath.includes('/')) {
    return `${componentPath}/index`
  }
  
  // 否则直接返回路径（如 /system/menu 返回 system/menu/index）
  return `${componentPath}/index`
}

/**
 * 动态导入组件
 * @param component 组件路径
 * @returns 组件
 */
export const loadComponent = (component: string) => {
  // 如果是Layout组件，直接返回
  if (component === 'Layout') {
    return Layout
  }
  
  // 如果组件路径为空，返回404组件
  if (!component) {
    return () => import('../views/error/404.vue')
  }
  
  // 处理组件路径格式
  // 1. 移除开头的 '/' 和结尾的 '.vue'
  let path = component.replace(/^\//, '').replace(/\.vue$/, '')
  
  // 2. 确保路径不包含 'views/'
  if (path.startsWith('views/')) {
    path = path.substring(6)
  }
  
  // 3. 动态导入组件，添加错误处理
  return () => {
    // 确保路径以 / 开头
    const importPath = `../views/${path}.vue`
    
    return import(importPath).catch(error => {
      ElMessage.error(`加载组件失败: ${path}`)
      
      // 尝试使用备用路径
      const backupPath = `../views/${path}/index.vue`
      
      return import(backupPath).catch(backupError => {
        ElMessage.error(`加载组件失败，备用路径也无效: ${path}/index`)
        return import('../views/error/404.vue')
      })
    })
  }
}

/**
 * 将菜单数据转换为路由配置
 * @param menus 菜单数据
 * @returns 路由配置
 */
export const generateRoutes = (menus: MenuItem[]): RouteRecordRaw[] => {
  const routes: RouteRecordRaw[] = []
  
  menus.forEach(menu => {
    // 跳过没有路径的菜单
    if (!menu.path) return
    
    // 创建路由配置
    const route: any = {
      path: menu.path,
      name: menu.name,
      meta: {
        title: menu.meta?.title || menu.name,
        icon: menu.meta?.icon || menu.icon,
        requiresAuth: menu.meta?.requiresAuth ?? true,
        keepAlive: menu.meta?.keepAlive ?? false,
        hidden: menu.isHidden
      }
    }
    
    // 如果有子菜单，则使用Layout作为组件
    if (menu.children && menu.children.length > 0) {
      route.component = Layout
      route.redirect = menu.children[0].path
      route.children = generateRoutes(menu.children)
    } else {
      // 如果没有指定组件路径，则根据菜单路径推断
      let componentPath = menu.component || inferComponentPath(menu.path)
      
      // 确保组件路径不以 / 开头，因为动态导入时会自动添加 ../views/
      if (componentPath.startsWith('/')) {
        componentPath = componentPath.substring(1)
      }
      
      route.component = loadComponent(componentPath)
    }
    
    routes.push(route)
  })
  
  return routes
}

/**
 * 过滤掉没有组件的菜单项（仅用于显示的菜单）
 * @param menus 菜单数据
 * @returns 过滤后的菜单数据
 */
export const filterMenus = (menus: MenuItem[]): MenuItem[] => {
  return menus.filter(menu => {
    // 如果有子菜单，则递归过滤
    if (menu.children && menu.children.length > 0) {
      menu.children = filterMenus(menu.children)
      return menu.children.length > 0
    }
    
    // 所有菜单都可以显示，因为我们会自动推断组件路径
    return true
  })
}

/**
 * 获取所有路由的名称，用于权限控制
 * @param routes 路由配置
 * @returns 路由名称数组
 */
export const getRouteNames = (routes: RouteRecordRaw[]): string[] => {
  const names: string[] = []
  
  routes.forEach(route => {
    if (route.name) {
      names.push(route.name.toString())
    }
    
    if (route.children) {
      names.push(...getRouteNames(route.children))
    }
  })
  
  return names
}

export default {
  generateRoutes,
  filterMenus,
  getRouteNames
} 