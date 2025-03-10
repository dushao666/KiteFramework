<template>
  <div class="nav-menu">
    <el-menu :default-active="activeMenu" class="el-menu-vertical" background-color="#304156" text-color="#bfcbd9"
      active-text-color="#409EFF" :collapse="isCollapse" router>
      <div class="logo-container">
        <span class="logo-title" v-show="!isCollapse">{{ NAMES.APP_NAME }}</span>
        <el-icon class="toggle-icon" @click="toggleCollapse">
          <Fold v-if="!isCollapse" />
          <Expand v-else />
        </el-icon>
      </div>

      <el-menu-item index="/home">
        <el-icon>
          <HomeFilled />
        </el-icon>
        <template #title>首页</template>
      </el-menu-item>

      <el-menu-item index="/import">
        <el-icon>
          <Document />
        </el-icon>
        <template #title>导入明细</template>
      </el-menu-item>

      <!-- <el-menu-item index="history">
        <el-icon>
          <List />
        </el-icon>
        <template #title>导入历史</template>
      </el-menu-item> -->
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { HomeFilled, Document, List, Fold, Expand } from '@element-plus/icons-vue'
import { NAMES } from '../../constants/index'
const route = useRoute()
const activeMenu = ref(route.path)
const isCollapse = ref(false)

watch(
  () => route.path,
  (newPath) => {
    activeMenu.value = newPath
  }
)

const emit = defineEmits(['update:is-collapse'])

const toggleCollapse = () => {
  isCollapse.value = !isCollapse.value
  emit('update:is-collapse', isCollapse.value)
}
</script>

<style scoped lang="scss">
.nav-menu {
  height: 100%;

  .el-menu-vertical {
    height: 100%;
    border-right: none;

    &:not(.el-menu--collapse) {
      width: 200px;
    }
  }
}

.logo-container {
  height: 60px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #2b2f3a;

  .logo-title {
    color: #fff;
    font-size: 16px;
    font-weight: 600;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .toggle-icon {
    color: #bfcbd9;
    font-size: 20px;
    cursor: pointer;
    transition: color 0.3s;

    &:hover {
      color: #fff;
    }
  }
}

:deep(.el-menu-item) {
  &.is-active {
    background-color: #263445 !important;
  }

  &:hover {
    background-color: #263445 !important;
  }
}

:deep(.el-menu-item) {
  .el-icon {
    margin-right: 16px;
    font-size: 18px;
  }
}
</style>