import { createRouter, createWebHashHistory, RouteRecordRaw } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import Layout from '../components/layout/index.vue'
import { getUserMenus } from '../api/menu'
import { generateRoutes } from './dynamicRoutes'
import { ElMessage } from 'element-plus'

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
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/404'
  }
]

// 动态路由，需要根据用户权限动态添加
export const asyncRoutes: Array<RouteRecordRaw> = []

const router = createRouter({
  history: createWebHashHistory(),
  routes: constantRoutes
})

// 是否已经添加了动态路由
let dynamicRoutesAdded = false

// 路由守卫
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
        
        // 重定向到目标页面，确保动态路由已加载
        return next({ ...to, replace: true })
      }
    } catch (error) {
      // 加载动态路由失败
      ElMessage.error('加载菜单失败，请刷新页面重试')
      return next('/home')
    }
  }
  
  // 已登录且已加载动态路由，直接放行
  next()
})

// 全局错误处理
router.onError((error) => {
  // 路由错误处理
  const pattern = /Loading chunk (\d)+ failed/g
  const isChunkLoadFailed = error.message.match(pattern)
  const targetPath = router.currentRoute.value.path
  
  if (isChunkLoadFailed) {
    ElMessage.error('加载页面失败，请刷新重试')
    router.replace(targetPath)
  }
})

export default router