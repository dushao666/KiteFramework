<template>
    <template v-if="!item.children">
        <el-menu-item :index="resolvePath(item.path)">
            <el-icon v-if="item.icon">
                <component :is="item.icon" />
            </el-icon>
            <template #title>{{ item.name }}</template>
        </el-menu-item>
    </template>

    <el-sub-menu v-else :index="resolvePath(item.path)">
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
.el-menu-item .el-icon,
.el-sub-menu .el-icon {
    margin-right: 16px;
    width: 24px;
    text-align: center;
    font-size: 18px;
}
</style>