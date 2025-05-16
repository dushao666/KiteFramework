import { createApp } from 'vue'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import App from './App.vue'
import router from './router'
import { ElMessage } from 'element-plus'
import { initRoutes } from './utils/routeUtils'

// 创建应用实例
const app = createApp(App)

// 创建状态管理
const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)
app.use(pinia)

// 注册所有图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

// 使用路由和UI库
app.use(router)
app.use(ElementPlus)

// 全局错误处理
app.config.errorHandler = (err: any, vm: any, info: string) => {
  ElMessage.error(`应用错误: ${err.message || '未知错误'}`)
}

// 挂载应用并初始化
app.mount('#app')

// 初始化路由
initRoutes().catch(error => {
  ElMessage.error('路由初始化失败')
}) 