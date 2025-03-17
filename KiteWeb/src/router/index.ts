import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
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
  history: createWebHistory(),
  routes: constantRoutes
})

// 是否已经添加了动态路由
let dynamicRoutesAdded = false

// 路由守卫
router.beforeEach(async (to, from, next) => {
  console.log(`路由跳转: 从 ${from.path} 到 ${to.path}`)
  
  const userStore = useUserStore()
  const token = userStore.jwtAccessToken
  
  // 设置页面标题
  document.title = to.meta.title ? `${to.meta.title}` : 'Admin System'

  // 不需要登录的页面直接放行
  if (!to.meta.requiresAuth) {
    console.log(`页面 ${to.path} 不需要认证，直接放行`)
    return next()
  }
  
  // 未登录，跳转到登录页
  if (!token) {
    console.log(`用户未登录，重定向到登录页`)
    return next('/login')
  }
  
  // 已登录但未加载动态路由
  if (!dynamicRoutesAdded) {
    console.log(`动态路由未加载，开始加载...`)
    try {
      // 获取用户菜单
      const res = await getUserMenus()
      if (res.code === 200) {
        console.log('获取到用户菜单:', res.data)
        
        // 生成动态路由
        const accessRoutes = generateRoutes(res.data)
        console.log('生成的动态路由:', accessRoutes)
        
        // 添加动态路由
        accessRoutes.forEach(route => {
          console.log('添加路由:', route.path, '组件:', route.component)
          router.addRoute(route)
        })
        
        // 添加404路由（确保它是最后添加的）
        router.addRoute({
          path: '/:pathMatch(.*)*',
          redirect: '/404'
        })
        
        // 标记动态路由已添加
        dynamicRoutesAdded = true
        console.log(`动态路由加载完成，重定向到 ${to.path}`)
        
        // 重定向到目标页面，确保动态路由已加载
        return next({ ...to, replace: true })
      }
    } catch (error) {
      console.error('加载动态路由失败:', error)
      ElMessage.error('加载菜单失败，请刷新页面重试')
      return next('/home')
    }
  }
  
  // 已登录且已加载动态路由，直接放行
  console.log(`用户已登录且动态路由已加载，放行到 ${to.path}`)
  next()
})

// 全局错误处理
router.onError((error) => {
  console.error('路由错误:', error)
  const pattern = /Loading chunk (\d)+ failed/g
  const isChunkLoadFailed = error.message.match(pattern)
  const targetPath = router.currentRoute.value.path
  
  if (isChunkLoadFailed) {
    ElMessage.error('加载页面失败，请刷新重试')
    router.replace(targetPath)
  }
})

export default router