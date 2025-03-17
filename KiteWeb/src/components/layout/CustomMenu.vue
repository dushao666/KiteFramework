<template>
    <div class="custom-menu" :class="{ 'is-collapsed': isCollapse }">
        <div v-for="item in menuItems" :key="item.path" class="menu-item-wrapper">
            <!-- 没有子菜单的菜单项 -->
            <router-link v-if="!item.children || item.children.length === 0" :to="item.path" class="menu-item"
                :class="{ 'is-active': isActive(item.path) }">
                <el-icon v-if="item.icon" class="menu-icon">
                    <component :is="item.icon" />
                </el-icon>
                <span v-if="!isCollapse" class="menu-title">{{ item.name }}</span>
            </router-link>

            <!-- 有子菜单的菜单项 -->
            <div v-else class="submenu"
                :class="{ 'is-active': isSubMenuActive(item), 'is-open': openedMenus.includes(item.path) }">
                <div class="submenu-title" @click="toggleSubMenu(item.path)">
                    <el-icon v-if="item.icon" class="menu-icon">
                        <component :is="item.icon" />
                    </el-icon>
                    <span v-if="!isCollapse" class="menu-title">{{ item.name }}</span>
                    <el-icon v-if="!isCollapse" class="arrow-icon">
                        <ArrowDown />
                    </el-icon>
                </div>

                <!-- 子菜单内容 -->
                <div v-if="!isCollapse && openedMenus.includes(item.path)" class="submenu-content">
                    <router-link v-for="child in item.children" :key="child.path"
                        :to="resolvePath(item.path, child.path)" class="submenu-item"
                        :class="{ 'is-active': isActive(resolvePath(item.path, child.path)) }">
                        <el-icon v-if="child.icon" class="menu-icon">
                            <component :is="child.icon" />
                        </el-icon>
                        <span class="menu-title">{{ child.name }}</span>
                    </router-link>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute } from 'vue-router';
import { ArrowDown } from '@element-plus/icons-vue';
import type { MenuItem } from '../../api/menu';

const props = defineProps<{
    menuItems: MenuItem[];
    isCollapse: boolean;
}>();

const route = useRoute();
const openedMenus = ref<string[]>([]);

// 判断菜单项是否激活
const isActive = (path: string) => {
    return route.path === path;
};

// 判断子菜单是否有激活项
const isSubMenuActive = (item: MenuItem) => {
    if (!item.children) return false;

    return item.children.some(child => {
        const fullPath = resolvePath(item.path, child.path);
        return isActive(fullPath);
    });
};

// 解析子菜单路径
const resolvePath = (basePath: string, childPath: string) => {
    if (childPath.startsWith('/')) return childPath;
    return `${basePath}/${childPath}`.replace(/\/+/g, '/');
};

// 切换子菜单展开/折叠状态
const toggleSubMenu = (path: string) => {
    const index = openedMenus.value.indexOf(path);
    if (index > -1) {
        openedMenus.value.splice(index, 1);
    } else {
        openedMenus.value.push(path);
    }
};

// 初始化展开当前路径的父菜单
const initOpenedMenus = () => {
    props.menuItems.forEach(item => {
        if (item.children && item.children.some(child => {
            const fullPath = resolvePath(item.path, child.path);
            return isActive(fullPath);
        })) {
            if (!openedMenus.value.includes(item.path)) {
                openedMenus.value.push(item.path);
            }
        }
    });
};

// 初始化
initOpenedMenus();
</script>

<script lang="ts">
export default {
    name: 'CustomMenu'
}
</script>

<style scoped lang="scss">
.custom-menu {
    width: 220px;
    height: 100%;
    transition: width 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    overflow-y: auto;
    overflow-x: hidden;

    &.is-collapsed {
        width: 64px;
    }
}

.menu-item-wrapper {
    margin: 8px 0;
}

.menu-item,
.submenu-title {
    display: flex;
    align-items: center;
    height: 44px;
    padding: 0 16px;
    margin: 4px 8px;
    border-radius: 6px;
    cursor: pointer;
    color: #333;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);

    &:hover {
        background-color: #f4f0ff;
        color: #6b48ff;
    }

    &.is-active {
        background-color: #f4f0ff;
        color: #6b48ff;
    }
}

.menu-icon {
    font-size: 16px;
    width: 20px;
    text-align: center;
    margin-right: 8px;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.menu-title {
    flex: 1;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    transition: opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.arrow-icon {
    font-size: 12px;
    transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.submenu {
    &.is-active {
        >.submenu-title {
            color: #6b48ff;
        }
    }

    &.is-open {
        >.submenu-title {
            .arrow-icon {
                transform: rotate(180deg);
            }
        }
    }
}

.submenu-content {
    padding-left: 16px;
    overflow: hidden;
    transition: max-height 0.3s cubic-bezier(0.4, 0, 0.2, 1), opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    max-height: 500px;
    opacity: 1;
    transform: translateY(0);
    animation: slideDownFade 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    transform-origin: top center;
}

@keyframes slideDownFade {
    from {
        opacity: 0;
        transform: translateY(-15px) scaleY(0.95);
        max-height: 0;
    }

    to {
        opacity: 1;
        transform: translateY(0) scaleY(1);
        max-height: 500px;
    }
}

.submenu-item {
    display: flex;
    align-items: center;
    height: 40px;
    padding: 0 16px;
    margin: 4px 8px;
    border-radius: 4px;
    cursor: pointer;
    color: #333;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    animation: fadeInDown 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    animation-fill-mode: both;

    &:hover {
        background-color: #f4f0ff;
        color: #6b48ff;
    }

    &.is-active {
        background-color: #f4f0ff;
        color: #6b48ff;
    }
}

@keyframes fadeInDown {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* 为子菜单项添加级联延迟 */
.submenu-item:nth-child(1) {
    animation-delay: 0.05s;
}

.submenu-item:nth-child(2) {
    animation-delay: 0.1s;
}

.submenu-item:nth-child(3) {
    animation-delay: 0.15s;
}

.submenu-item:nth-child(4) {
    animation-delay: 0.2s;
}

.submenu-item:nth-child(5) {
    animation-delay: 0.25s;
}

.submenu-item:nth-child(6) {
    animation-delay: 0.3s;
}

a {
    text-decoration: none;
    color: inherit;
}
</style>