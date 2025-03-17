<template>
    <div class="breadcrumb">
        <el-breadcrumb separator="/">
            <el-breadcrumb-item v-for="(item, index) in breadcrumbs" :key="index">
                <span v-if="index === breadcrumbs.length - 1">{{ item.title }}</span>
                <router-link v-else :to="item.path">{{ item.title }}</router-link>
            </el-breadcrumb-item>
        </el-breadcrumb>
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute, RouteLocationNormalizedLoaded } from 'vue-router'

interface BreadcrumbItem {
    title: string
    path: string
}

const route = useRoute()
const breadcrumbs = ref<BreadcrumbItem[]>([])

// 生成面包屑导航
const generateBreadcrumbs = (route: RouteLocationNormalizedLoaded) => {
    const paths = route.path.split('/').filter(Boolean)
    const items: BreadcrumbItem[] = []
    
    // 添加首页
    items.push({
        title: '首页',
        path: '/home'
    })
    
    // 根据路径生成面包屑
    let currentPath = ''
    paths.forEach(path => {
        currentPath += `/${path}`
        const matched = route.matched.find(r => r.path === currentPath)
        if (matched && matched.meta.title) {
            items.push({
                title: matched.meta.title as string,
                path: currentPath
            })
        }
    })
    
    return items
}

// 监听路由变化
watch(() => route.path, () => {
    breadcrumbs.value = generateBreadcrumbs(route)
}, { immediate: true })
</script>

<script lang="ts">
export default {
    name: 'Breadcrumb'
}
</script>

<style scoped lang="scss">
.breadcrumb {
    display: flex;
    align-items: center;
    font-size: 14px;
    
    :deep(.el-breadcrumb__item) {
        .el-breadcrumb__inner {
            color: #606266;
            
            &.is-link {
                color: #909399;
                
                &:hover {
                    color: #409eff;
                }
            }
        }
        
        &:last-child {
            .el-breadcrumb__inner {
                color: #303133;
                font-weight: 600;
            }
        }
    }
}
</style>