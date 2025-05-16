import { RouteRecordRaw } from 'vue-router'
import Layout from '../components/layout/index.vue'

// 静态路由配置
const staticRoutes: Array<RouteRecordRaw> = [
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
  // 系统管理路由
  {
    path: '/system',
    component: Layout,
    name: 'System',
    meta: {
      title: '系统管理',
      requiresAuth: true
    },
    children: [
      {
        path: 'role',
        name: 'Role',
        component: () => import('../views/system/role/index.vue'),
        meta: {
          title: '角色管理',
          requiresAuth: true
        }
      },
      {
        path: 'menu',
        name: 'Menu',
        component: () => import('../views/system/menu/index.vue'),
        meta: {
          title: '菜单管理',
          requiresAuth: true
        }
      },
      {
        path: 'user',
        name: 'User',
        component: () => import('../views/system/user/index.vue'),
        meta: {
          title: '用户管理',
          requiresAuth: true
        }
      },
      {
        path: ':pathMatch(.*)*',
        component: () => import('../views/error/404.vue'),
        meta: {
          title: '404',
          requiresAuth: true
        }
      }
    ]
  },
  
  // 错误页面
  {
    path: '/404',
    name: '404',
    component: () => import('../views/error/404.vue'),
    meta: {
      title: '404',
      requiresAuth: false
    }
  },
  // 通用模块通配符路由 - 所有未知路径的一级路由会被重定向到这里
  {
    path: '/:module',
    component: Layout,
    children: [
      {
        path: ':pathMatch(.*)*',
        component: () => import('../views/error/404.vue')
      }
    ]
  },
  // 全局404路由必须放在最后
  {
    path: '/:pathMatch(.*)*',
    redirect: '/404'
  }
]

export default staticRoutes 