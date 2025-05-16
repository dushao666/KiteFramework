import { createRouter, createWebHashHistory, createWebHistory, RouteRecordRaw } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import { useRouteStore } from '../stores/routeStore'
import Layout from '../components/layout/index.vue'
import { getUserMenus } from '../api/menu'
import { MenuItem } from '../api/menu'
import { ElMessage } from 'element-plus'
import staticRoutes from './staticRoutes'
import { createDynamicRoute } from '../utils/routeUtils'

// ==================== 路由配置 ====================

// 静态路由，不需要权限控制
export const constantRoutes: Array<RouteRecordRaw> = [
  {
    path: '/',
    redirect: '/home'
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/login/index.vue'),
    meta: {
      title: '登录',
      requiresAuth: false
    }
  },
  {
    path: '/home',
    component: Layout,
    children: [
      {
        path: '',
        name: 'Home',
        component: () => import('../views/home/index.vue'),
        meta: {
          title: '首页',
          requiresAuth: true
        }
      }
    ]
  },
  {
    path: '/profile',
    component: Layout,
    children: [
      {
        path: '',
        name: 'Profile',
        component: () => import('../views/profile/index.vue'),
        meta: {
          title: '个人信息',
          requiresAuth: true
        }
      }
    ]
  },
  {
    path: '/404',
    name: '404',
    component: () => import('../views/error/404.vue'),
    meta: {
      title: '404',
      requiresAuth: false
    }
  }
]

// 重要：将404匹配路由单独定义，而不是放在静态路由中
export const notFoundRoute = {
  path: '/:pathMatch(.*)*',
  redirect: '/404'
}

// 动态路由，需要根据用户权限动态添加
export const asyncRoutes: Array<RouteRecordRaw> = []

// ==================== 组件加载处理 ====================

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
  
  // 推断逻辑处理
  if (componentPath.includes('/')) {
    // 尝试捕获常见的路由模式
    
    // 1. /module/action 模式 (如 /system/user)
    if (componentPath.split('/').length === 2) {
      // 两级路径，直接返回 module/action/index
      return `${componentPath}/index`
    }
    
    // 2. /module/action/subaction 模式 (如 /system/user/add)
    if (componentPath.split('/').length === 3) {
      const parts = componentPath.split('/')
      
      // 可能是详情、添加、编辑等页面，放在二级目录下
      if (['add', 'edit', 'detail', 'view'].includes(parts[2])) {
        return `${parts[0]}/${parts[1]}/${parts[2]}`
      }
      
      // 否则可能是独立功能，直接使用完整路径
      return componentPath
    }
    
    // 对于更复杂的路径，尝试智能推断
    return componentPath
  } else {
    // 单级路径 (如 /dashboard)
    return `${componentPath}/index`
  }
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
  let path = component.replace(/^\//, '').replace(/\.vue$/, '')
  
  // 确保路径不包含 'views/'
  if (path.startsWith('views/')) {
    path = path.substring(6)
  }
  
  // 动态导入组件
  return () => {
    // 多种尝试路径
    const tryPaths = [
      // 1. 直接使用提供的路径
      `../views/${path}.vue`,
      
      // 2. 添加 /index 后缀
      `../views/${path}/index.vue`,
      
      // 3. 对于系统模块的常规格式 (system/xxx)
      path.includes('/') ? `../views/${path.split('/')[0]}/${path.split('/')[1]}/index.vue` : null,
      
      // 4. 对于自定义模块，尝试常见的文件结构
      path.includes('/') ? `../views/${path.replace(/\/\w+$/, '')}/index.vue` : null
    ].filter(Boolean) as string[];
    
    // 递归尝试所有可能的路径
    const tryNextPath = (pathIndex: number) => {
      if (pathIndex >= tryPaths.length) {
        return import('../views/error/404.vue');
      }
      
      const currentPath = tryPaths[pathIndex];
      
      return import(/* @vite-ignore */ currentPath)
        .then(component => {
          return component;
        })
        .catch(error => {
          return tryNextPath(pathIndex + 1);
        });
    };
    
    return tryNextPath(0);
  }
}

// ==================== 路由生成工具 ====================

/**
 * 将菜单数据转换为路由配置
 * @param menus 菜单数据
 * @returns 路由配置
 */
export const generateRoutes = (menus: MenuItem[]): RouteRecordRaw[] => {
  const routes: RouteRecordRaw[] = []
  
  menus.forEach(menu => {
    // 跳过没有路径的菜单
    if (!menu.path) {
      return
    }
    
    // 创建路由配置
    const route: any = {
      path: menu.path,
      name: menu.name || menu.path.replace(/\//g, '-').replace(/^-/, ''),
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
      
      // 确保组件路径不以 / 开头
      if (componentPath.startsWith('/')) {
        componentPath = componentPath.substring(1)
      }
      
      // 如果组件路径没有文件扩展名，检查是否为Layout
      if (componentPath === 'Layout') {
        route.component = Layout
      } else {
        route.component = loadComponent(componentPath)
      }
    }
    
    routes.push(route)
  })
  
  return routes
}

/**
 * 过滤掉没有组件的菜单项
 * @param menus 菜单数据
 * @returns 过滤后的菜单数据
 */
export const filterMenus = (menus: MenuItem[]): MenuItem[] => {
  return menus.filter(menu => {
    if (menu.children && menu.children.length > 0) {
      menu.children = filterMenus(menu.children)
      return menu.children.length > 0
    }
    return true
  })
}

// ==================== 路由实例创建 ====================

// 创建路由实例，使用预定义的静态路由
const router = createRouter({
  history: createWebHistory(), // 使用历史模式，URL中不包含#，但需要服务器配置支持URL重写
  routes: staticRoutes
})

// ==================== 动态路由控制 ====================

// 是否已经添加了动态路由
let dynamicRoutesAdded = false

/**
 * 设置动态路由状态
 * @param value 状态值
 */
export const setDynamicRoutesAdded = (value: boolean) => {
  dynamicRoutesAdded = value
  const routeStore = useRouteStore()
  routeStore.setDynamicRoutesAdded(value)
}

/**
 * 重置路由
 * @returns 是否重置成功
 */
export const resetRouter = async (): Promise<boolean> => {
  const newRouter = createRouter({
    history: createWebHistory(),
    routes: staticRoutes 
  })
  
  // @ts-ignore: 替换路由器的matcher
  router.matcher = newRouter.matcher
  
  // 重置动态路由状态
  dynamicRoutesAdded = false
  
  // 更新持久化状态
  const routeStore = useRouteStore()
  routeStore.resetRouteState()
  
  return true
}

/**
 * 加载用户菜单并生成动态路由
 * @returns 是否加载成功
 */
export const loadUserMenus = async (): Promise<boolean> => {
  try {
    // 获取用户菜单
    const res = await getUserMenus()
    
    if (!res || res.code !== 200) {
      ElMessage.error(`获取菜单失败: ${res?.message || '未知错误'}`)
      return false
    }
    
    // 检查菜单数据
    if (!res.data || !Array.isArray(res.data) || res.data.length === 0) {
      // 菜单数据为空，将只使用静态路由
    } else {
      // 生成动态路由
      const accessRoutes = generateRoutes(res.data)
      
      // 添加动态路由
      accessRoutes.forEach(route => {
        router.addRoute(route)
      })
    }
    
    // 添加404路由（确保它是最后添加的）
    router.addRoute(notFoundRoute)
    
    // 打印所有路由以检查
    const allRoutes = router.getRoutes()
    const routePaths = allRoutes.map(r => r.path)
    
    // 更新路由状态
    const routeStore = useRouteStore()
    routeStore.setAllRoutes(routePaths)
    routeStore.setMenuLoaded(true)
    dynamicRoutesAdded = true
    routeStore.setDynamicRoutesAdded(true)
    
    return true
  } catch (error: any) {
    ElMessage.error(`加载菜单失败: ${error.message || '未知错误'}`)
    return false
  }
}

// ==================== 路由守卫 ====================

router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore()
  const routeStore = useRouteStore()
  const token = userStore.jwtAccessToken
  
  // 设置页面标题
  document.title = to.meta.title ? `${to.meta.title}` : 'Admin System'

  // 不需要登录的页面直接放行
  if (!to.meta.requiresAuth) {
    return next()
  }
  
  // 未登录，跳转到登录页
  if (!token) {
    return next('/login')
  }
  
  // 从localStorage恢复路由状态
  if (routeStore.getDynamicRoutesAdded && !dynamicRoutesAdded) {
    dynamicRoutesAdded = true
  }
  
  // 检查要访问的路由是否存在
  const allRoutes = router.getRoutes()
  const routeExists = checkRouteExists(to.path, allRoutes)
  
  // 如果路由不存在或者路由状态过期，尝试重新加载
  if ((!routeExists || routeStore.isMenuExpired) && to.name !== '404') {
    try {
      // 重置路由
      await resetRouter()
      
      // 重新加载用户菜单和路由
      const success = await loadUserMenus()
      if (success) {
        // 再次检查路由是否存在
        const newRouteExists = checkRouteExists(to.path, router.getRoutes())
        if (!newRouteExists) {
          ElMessage.warning('页面组件不存在，请联系管理员')
        }
        
        return next({ ...to, replace: true })
      } else {
        return next('/home')
      }
    } catch (error) {
      return next('/home')
    }
  }
  
  // 已登录但未加载菜单和路由
  if (!dynamicRoutesAdded) {
    try {
      const success = await loadUserMenus()
      if (success) {
        return next({ ...to, replace: true })
      } else {
        ElMessage.error('菜单加载失败，将跳转到首页')
        return next('/home')
      }
    } catch (error) {
      ElMessage.error('菜单加载失败，请重新登录')
      userStore.logout()
      return next('/login')
    }
  }
  
  // 已登录且已加载菜单和路由
  next()
})

/**
 * 检查路由是否存在
 * @param path 路径
 * @param routes 路由列表
 * @returns 是否存在
 */
function checkRouteExists(path: string, routes: RouteRecordRaw[]): boolean {
  // 精确匹配
  if (routes.some(route => route.path === path)) {
    return true
  }
  
  // 对于嵌套路由的处理
  if (path.includes('/')) {
    const segments = path.split('/')
    
    // 检查是否有匹配的父路由
    for (let i = 1; i <= segments.length; i++) {
      const parentPath = segments.slice(0, i).join('/')
      
      // 找到匹配的父路由
      const parentRoute = routes.find(r => r.path === parentPath)
      if (parentRoute && parentRoute.children) {
        // 递归检查子路由
        const childPath = segments.slice(i).join('/')
        return checkRouteExists(childPath, parentRoute.children as RouteRecordRaw[])
      }
    }
    
    // 检查带参数的路由
    return routes.some(route => {
      if (route.path.includes('/:')) {
        const routeBase = route.path.split('/:')[0]
        return path.startsWith(routeBase)
      }
      return false
    })
  }
  
  return false
}

// 全局错误处理
router.onError((error) => {
  console.error('路由错误:', error)
  const pattern = /Loading chunk (\d)+ failed/g
  const isChunkLoadFailed = error.message.match(pattern)
  
  if (isChunkLoadFailed) {
    console.error('加载页面失败:', error.message)
    ElMessage.error('加载页面失败，请刷新重试')
    router.replace(router.currentRoute.value.path)
  }
})

export default router