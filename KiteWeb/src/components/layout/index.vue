<template>
    <div class="app-wrapper">
        <!-- 侧边栏 -->
        <div class="sidebar-container" :class="{ 'is-collapsed': isCollapse }">
            <div class="logo-container">
                <img src="../../assets/logo.png" alt="Logo" class="logo" />
                <span class="title" v-if="!isCollapse">{{ NAMES.APP_NAME }}</span>
            </div>

            <el-scrollbar>
                <el-menu :default-active="activeMenu" :collapse="isCollapse" :unique-opened="true"
                    background-color="#304156" text-color="#bfcbd9" active-text-color="#409EFF" router
                    class="sidebar-menu">
                    <SidebarItem v-for="menu in menuList" :key="menu.path" :item="menu" :base-path="menu.path" />
                </el-menu>
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../../stores/userStore'
import type { MenuItem } from '../../api/menu'
import { getMenuTree } from '../../api/menu'
import { NAMES } from '../../constants'
import Breadcrumb from './Breadcrumb.vue'
import SidebarItem from './SidebarItem.vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Fold, Expand, ArrowDown } from '@element-plus/icons-vue'

const router = useRouter()
const userStore = useUserStore()
const isCollapse = ref(false)
const menuList = ref<MenuItem[]>([])
const username = computed(() => userStore.userInfo?.username || '用户')

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
        const res = await getMenuTree()
        if (res.code === 200) {
            menuList.value = res.data
        } else {
            // 如果API返回错误，使用模拟数据
            menuList.value = [
                {
                    id: 1,
                    name: '仪表盘',
                    path: '/dashboard',
                    icon: 'Odometer',
                    children: []
                },
                {
                    id: 2,
                    name: '系统管理',
                    path: '/system',
                    icon: 'Setting',
                    children: [
                        {
                            id: 3,
                            name: '菜单管理',
                            path: '/system/menu',
                            icon: 'Menu',
                            parentId: 2
                        }
                    ]
                }
            ]
        }
    } catch (error) {
        console.error('获取菜单数据失败:', error)
        menuList.value = [
            {
                id: 1,
                name: '仪表盘',
                path: '/dashboard',
                icon: 'Odometer',
                children: []
            }
        ]
    }
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
        await userStore.logout()
        router.push('/login')
        ElMessage.success('退出登录成功')
    }).catch(() => { })
}

onMounted(() => {
    getMenuData()
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
    width: 210px;
    height: 100%;
    background-color: #304156;
    transition: width 0.3s;
    position: relative;
    z-index: 1001;
    box-shadow: 2px 0 6px rgba(0, 21, 41, 0.15);

    &.is-collapsed {
        width: 64px;
    }
}

.logo-container {
    height: 60px;
    padding: 10px;
    display: flex;
    align-items: center;
    background-color: #263445;
    position: relative;
    overflow: hidden;

    .logo {
        width: 32px;
        height: 32px;
        margin-right: 10px;
    }

    .title {
        color: #fff;
        font-size: 16px;
        font-weight: bold;
        white-space: nowrap;
        overflow: hidden;
    }
}

/* 修改滚动条样式 */
:deep(.el-scrollbar) {
    height: calc(100% - 60px);
    background-color: #304156;
}

:deep(.el-scrollbar__wrap) {
    overflow-x: hidden !important;
}

/* 菜单样式调整 */
:deep(.sidebar-menu) {
    border-right: none;
    height: 100%;
}

:deep(.el-menu) {
    border-right: none;
}

:deep(.el-menu-item),
:deep(.el-sub-menu__title) {
    &:hover {
        background-color: #263445 !important;
    }
}

:deep(.el-menu-item.is-active) {
    background-color: #263445 !important;
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

/* 确保子菜单弹出层样式正确 */
:deep(.el-menu--popup) {
    background-color: #304156 !important;

    .el-menu-item {
        background-color: #304156;
        color: #bfcbd9;

        &:hover,
        &.is-active {
            background-color: #263445 !important;
            color: #409EFF;
        }
    }
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
</style>