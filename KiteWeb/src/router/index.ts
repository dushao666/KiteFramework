import { createRouter, createWebHashHistory, RouteRecordRaw } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import Layout from '../components/layout/index.vue'
import { getUserMenus } from '../api/menu'
import { MenuItem } from '../api/menu'
import { ElMessage } from 'element-plus'

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
  
  // 如果路径中没有子路径（如 /home），则返回 home/index
  if (!componentPath.includes('/')) {
    return `${componentPath}/index`
  }
  
  // 返回路径（如 /system/menu 返回 system/menu/index）
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
  let path = component.replace(/^\//, '').replace(/\.vue$/, '')
  
  // 确保路径不包含 'views/'
  if (path.startsWith('views/')) {
    path = path.substring(6)
  }
  
  // 动态导入组件
  return () => {
    const importPath = `../views/${path}.vue`
    const backupPath = `../views/${path}/index.vue`
    
    return new Promise((resolve, reject) => {
      import(/* @vite-ignore */ importPath)
        .then(component => resolve(component))
        .catch(() => {
          import(/* @vite-ignore */ backupPath)
            .then(component => resolve(component))
            .catch(error => {
              console.error(`组件加载失败: ${path}`, error)
              import('../views/error/404.vue')
                .then(component => resolve(component))
                .catch(e => reject(e))
            })
        })
    })
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
      
      // 确保组件路径不以 / 开头
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

const router = createRouter({
  history: createWebHashHistory(),
  routes: constantRoutes
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
}

/**
 * 重置路由
 * @returns 是否重置成功
 */
export const resetRouter = async (): Promise<boolean> => {
  const newRouter = createRouter({
    history: createWebHashHistory(),
    routes: constantRoutes
  })
  
  // @ts-ignore: 替换路由器的matcher
  router.matcher = newRouter.matcher
  
  // 重置动态路由状态
  dynamicRoutesAdded = false
  
  return true
}

/**
 * 加载动态路由
 * @returns 是否加载成功
 */
export const loadDynamicRoutes = async (): Promise<boolean> => {
  try {
    // 获取用户菜单
    const res = await getUserMenus()
    if (res.code === 200) {
      // 生成动态路由
      const accessRoutes = generateRoutes(res.data)
      
      // 添加动态路由
      accessRoutes.forEach(route => {
        router.addRoute(route)
      })
      
      // 添加404路由（确保它是最后添加的）
      router.addRoute({
        path: '/:pathMatch(.*)*',
        redirect: '/404'
      })
      
      // 标记动态路由已添加
      dynamicRoutesAdded = true
      
      return true
    }
    return false
  } catch (error) {
    console.error('加载动态路由失败:', error)
    ElMessage.error('加载菜单失败，请刷新页面重试')
    return false
  }
}

// ==================== 路由守卫 ====================

router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore()
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
  
  // 已登录但未加载动态路由
  if (!dynamicRoutesAdded) {
    try {
      const success = await loadDynamicRoutes()
      if (success) {
        // 重定向到目标页面，确保动态路由已加载
        return next({ ...to, replace: true })
      } else {
        return next('/home')
      }
    } catch (error) {
      console.error('路由加载失败:', error)
      ElMessage.error('路由加载失败，请重新登录')
      return next('/login')
    }
  }
  
  // 已登录且已加载动态路由，直接放行
  next()
})

// 全局错误处理
router.onError((error) => {
  console.error('路由错误:', error)
  const pattern = /Loading chunk (\d)+ failed/g
  const isChunkLoadFailed = error.message.match(pattern)
  
  if (isChunkLoadFailed) {
    ElMessage.error('加载页面失败，请刷新重试')
    router.replace(router.currentRoute.value.path)
  }
})

export default router