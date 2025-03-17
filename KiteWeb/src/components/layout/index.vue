<template>
    <div class="app-wrapper">
        <!-- 侧边栏 -->
        <div class="sidebar-container" :class="{ 'is-collapsed': isCollapse }">
            <div class="logo-container">
                <img src="../../assets/logo.png" alt="Logo" class="logo" />
                <span class="title">{{ NAMES.APP_NAME }}</span>
            </div>

            <el-scrollbar>
                <CustomMenu :menu-items="menuList" :is-collapse="isCollapse" />
            </el-scrollbar>
        </div>

        <!-- 主要内容区 -->
        <div class="main-container">
            <!-- 顶部导航栏 -->
            <div class="navbar">
                <div class="left-menu">
                    <el-icon class="fold-icon" @click="toggleSidebar">
                        <Fold v-if="!isCollapse" />
                        <Expand v-else />
                    </el-icon>
                    <Breadcrumb />
                </div>

                <div class="right-menu">
                    <el-dropdown trigger="click">
                        <div class="avatar-container">
                            <img src="../../assets/avatar.png" class="avatar" />
                            <span class="username">{{ username }}</span>
                            <el-icon>
                                <ArrowDown />
                            </el-icon>
                        </div>
                        <template #dropdown>
                            <el-dropdown-menu>
                                <el-dropdown-item @click="goToProfile">个人信息</el-dropdown-item>
                                <el-dropdown-item divided @click="handleLogout">退出登录</el-dropdown-item>
                            </el-dropdown-menu>
                        </template>
                    </el-dropdown>
                </div>
            </div>

            <!-- 内容区 -->
            <div class="app-main">
                <router-view v-slot="{ Component }">
                    <transition name="fade-transform" mode="out-in">
                        <keep-alive>
                            <component :is="Component" />
                        </keep-alive>
                    </transition>
                </router-view>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../../stores/userStore'
import type { MenuItem } from '../../api/menu'
import { getMenuTree, getUserMenus } from '../../api/menu'
import { NAMES } from '../../constants'
import Breadcrumb from './Breadcrumb.vue'
import CustomMenu from './CustomMenu.vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Fold, Expand, ArrowDown } from '@element-plus/icons-vue'

const router = useRouter()
const userStore = useUserStore()
const isCollapse = ref(false)
const menuList = ref<MenuItem[]>([])
const username = computed(() => userStore.username || '用户')

// 获取当前激活的菜单
const activeMenu = computed(() => {
    const route = router.currentRoute.value
    return route.path
})

// 切换侧边栏折叠状态
const toggleSidebar = () => {
    isCollapse.value = !isCollapse.value
}

// 获取菜单数据
const getMenuData = async () => {
    try {
        // 使用getUserMenus获取当前用户的菜单
        const res = await getUserMenus()
        if (res.code === 200) {
            // 过滤掉隐藏的菜单
            menuList.value = res.data.filter(item => !item.isHidden)
        } else {
            // 如果API返回错误，使用模拟数据
            menuList.value = [
                {
                    id: 1,
                    name: '首页',
                    path: '/home',
                    icon: 'HomeFilled',
                    children: []
                }
            ]
        }
    } catch (error) {
        console.error('获取菜单数据失败:', error)
        menuList.value = [
            {
                id: 1,
                name: '首页',
                path: '/home',
                icon: 'HomeFilled',
                children: []
            }
        ]
    }
}

// 监听菜单刷新事件
const handleRefreshMenu = () => {
    getMenuData()
}

// 跳转到个人信息页面
const goToProfile = () => {
    router.push('/profile')
}

// 处理退出登录
const handleLogout = () => {
    ElMessageBox.confirm('确定要退出登录吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(async () => {
        // 调用 userStore 的 logout 方法
        userStore.$reset()
        localStorage.removeItem('userState')
        router.push('/login')
        ElMessage.success('退出登录成功')
    }).catch(() => { })
}

onMounted(() => {
    getMenuData()
    // 添加菜单刷新事件监听
    window.addEventListener('refresh-menu', handleRefreshMenu)
})

onBeforeUnmount(() => {
    // 移除事件监听器
    window.removeEventListener('refresh-menu', handleRefreshMenu)
})
</script>

<style scoped lang="scss">
.app-wrapper {
    display: flex;
    height: 100vh;
    width: 100%;
    position: relative;
    overflow: hidden;
}

.sidebar-container {
    width: 220px;
    height: 100%;
    background-color: #fff;
    transition: width 0.3s ease;
    position: relative;
    z-index: 1001;
    box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
    overflow: hidden;

    &.is-collapsed {
        width: 64px;

        .logo-container {
            .title {
                width: 0;
                opacity: 0;
            }
        }
    }
}

.logo-container {
    height: 56px;
    padding: 0 16px;
    display: flex;
    align-items: center;
    background-color: #fff;
    border-bottom: 1px solid #f0f0f0;
    overflow: hidden;

    .logo {
        height: 28px;
        width: 28px;
        flex-shrink: 0;
    }

    .title {
        margin-left: 8px;
        color: #1a1a1a;
        font-size: 15px;
        font-weight: 600;
        white-space: nowrap;
        opacity: 1;
        width: auto;
        transition: all 0.3s ease;
    }
}

/* 修改滚动条样式 */
:deep(.el-scrollbar) {
    height: calc(100% - 56px);
    background-color: #fff;
}

:deep(.el-scrollbar__wrap) {
    overflow-x: hidden !important;
}

/* 主容器样式 */
.main-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    min-width: 0;
    position: relative;
    z-index: 1000;
}

.navbar {
    height: 60px;
    background-color: #fff;
    position: relative;
    box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 20px;
}

.left-menu {
    display: flex;
    align-items: center;
}

.fold-icon {
    font-size: 20px;
    cursor: pointer;
    margin-right: 15px;
    color: #606266;
}

.right-menu {
    display: flex;
    align-items: center;
}

.avatar-container {
    display: flex;
    align-items: center;
    cursor: pointer;
}

.avatar {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    margin-right: 8px;
}

.username {
    font-size: 14px;
    color: #606266;
    margin-right: 5px;
}

.app-main {
    flex: 1;
    padding: 20px;
    overflow-y: auto;
    background-color: #f5f7fa;
}

/* 修复过渡动画 */
.fade-transform-enter-active,
.fade-transform-leave-active {
    transition: all 0.3s;
}

.fade-transform-enter-from,
.fade-transform-leave-to {
    opacity: 0;
    transform: translateX(20px);
}

/* 修改菜单过渡效果 */
:deep(.el-menu) {
    --el-transition-duration: 0s !important;
    --el-menu-transition-duration: 0s !important;
}

:deep(.el-sub-menu) {
    .el-sub-menu__title {
        transition: background-color 0.3s, color 0.3s !important;
    }

    /* 禁用子菜单的高度过渡 */
    .el-menu--inline {
        transition: none !important;
        padding: 0 !important;

        /* 为子菜单项添加自定义过渡 */
        .el-menu-item {
            transition: background-color 0.3s, color 0.3s !important;
            padding-left: 40px !important;
            height: 40px;
            line-height: 40px;
            margin: 4px 8px;
        }
    }
}

/* 只保留箭头的过渡 */
:deep(.el-sub-menu__icon-arrow) {
    transition: transform 0.3s !important;
}

/* 禁用折叠菜单的过渡 */
:deep(.el-menu--collapse) {
    transition: width 0.3s !important;
}
</style>