<template>
    <template v-if="!item.children">
        <el-menu-item :index="resolvePath(item.path)">
            <el-icon v-if="item.icon">
                <component :is="item.icon" />
            </el-icon>
            <template #title>{{ item.name }}</template>
        </el-menu-item>
    </template>

    <el-sub-menu v-else :index="resolvePath(item.path)" :popper-append-to-body="false">
        <template #title>
            <el-icon v-if="item.icon">
                <component :is="item.icon" />
            </el-icon>
            <span>{{ item.name }}</span>
        </template>

        <sidebar-item v-for="child in item.children" :key="child.path" :item="child"
            :base-path="resolvePath(item.path)" />
    </el-sub-menu>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import path from 'path-browserify'
import type { MenuItem } from '../../api/menu'

const props = defineProps<{
    item: MenuItem
    basePath: string
}>()

// 解析路由路径
const resolvePath = (routePath: string) => {
    if (path.isAbsolute(routePath)) {
        return routePath
    }
    return path.resolve(props.basePath, routePath)
}
</script>

<style scoped>
/* 完全禁用过渡效果 */
:deep(.el-sub-menu) {
    .el-sub-menu__title {
        transition: background-color 0.3s, color 0.3s !important;
    }

    .el-menu {
        transition: none !important;
        border: none !important;
    }
}

.el-menu-item .el-icon,
.el-sub-menu .el-icon {
    margin-right: 16px;
    width: 24px;
    text-align: center;
    font-size: 18px;
}
</style>