import { defineStore } from 'pinia'

export const useRouteStore = defineStore('route', {
  persist: {
    key: 'routeState',
    storage: localStorage,
  },

  state: () => ({
    dynamicRoutesAdded: false,
    menuLoaded: false,
    lastLoadTime: 0,
    allRoutes: [] as string[],
  }),

  actions: {
    // 设置动态路由状态
    setDynamicRoutesAdded(value: boolean) {
      this.dynamicRoutesAdded = value
      this.lastLoadTime = Date.now()
    },

    // 保存菜单加载状态
    setMenuLoaded(value: boolean) {
      this.menuLoaded = value
      this.lastLoadTime = Date.now()
    },
    
    // 保存路由列表
    setAllRoutes(routes: string[]) {
      this.allRoutes = routes
    },

    // 重置路由状态
    resetRouteState() {
      this.dynamicRoutesAdded = false
      this.menuLoaded = false
      this.lastLoadTime = 0
      this.allRoutes = []
    }
  },

  getters: {
    // 获取动态路由状态
    getDynamicRoutesAdded: (state) => state.dynamicRoutesAdded,
    
    // 获取菜单加载状态
    getMenuLoaded: (state) => state.menuLoaded,
    
    // 获取所有路由
    getAllRoutes: (state) => state.allRoutes,
    
    // 检查菜单是否已过期（超过1小时）
    isMenuExpired: (state) => {
      const oneHour = 60 * 60 * 1000
      return Date.now() - state.lastLoadTime > oneHour
    }
  },
}) 