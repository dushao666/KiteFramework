<template>
    <el-breadcrumb class="breadcrumb">
        <el-breadcrumb-item v-for="(item, index) in breadcrumbs" :key="index" :to="item.path">
            {{ item.title }}
        </el-breadcrumb-item>
    </el-breadcrumb>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const breadcrumbs = ref<Array<{ title: string; path: string }>>([])

// 更新面包屑
const updateBreadcrumbs = () => {
    const matched = route.matched.filter(item => item.meta && item.meta.title)
    breadcrumbs.value = matched.map(item => ({
        title: item.meta.title as string,
        path: item.path
    }))
}

// 监听路由变化
watch(
    () => route.path,
    () => updateBreadcrumbs(),
    { immediate: true }
)
</script>

<style scoped>
.breadcrumb {
    margin-left: 8px;
    line-height: 60px;
}
</style>