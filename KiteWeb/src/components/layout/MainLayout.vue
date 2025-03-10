<template>
    <el-container class="layout-container">
        <!-- 左侧菜单 -->
        <el-aside :width="isCollapse ? '64px' : '200px'" class="aside-container">
            <nav-menu v-model:is-collapse="isCollapse" @update:is-collapse="handleCollapse" />
        </el-aside>

        <el-container class="main-container">
            <!-- 顶部头部 -->
            <el-header>
                <app-header />
            </el-header>

            <!-- 主要内容区域 -->
            <el-main>
                <slot></slot>
            </el-main>
        </el-container>
    </el-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import NavMenu from './NavMenu.vue'
import AppHeader from './AppHeader.vue'

const isCollapse = ref(false)

const handleCollapse = (value: boolean) => {
    isCollapse.value = value
}
</script>

<style scoped lang="scss">
.layout-container {
    height: 100vh;
    display: flex;

    .aside-container {
        transition: width 0.3s;
        overflow: hidden;
        flex-shrink: 0;
        background-color: #304156;
        box-shadow: 0 1px 4px rgba(0, 21, 41, .08);
    }

    .main-container {
        flex: 1;
        min-width: 0; // 防止内容溢出
        transition: all 0.3s;

        .el-header {
            height: 60px;
            background-color: #fff;
            border-bottom: 1px solid #dcdfe6;
            padding: 0;
            box-shadow: 0 1px 4px rgba(0, 21, 41, .08);
        }

        .el-main {
            background-color: #f0f2f5;
            padding: 0;
            overflow-y: auto;
        }
    }
}
</style>