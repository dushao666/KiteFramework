<template>
  <div class="app-header">
    <div class="header-left">
      <h2></h2>
    </div>
    <div class="header-right">
      <el-dropdown @command="handleCommand" class="user-dropdown">
        <div class="user-info">
          <span class="username">{{ userStore.getUsername || '未登录' }}</span>
          <el-icon class="dropdown-icon">
            <CaretBottom />
          </el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="logout">退出登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useUserStore } from '../../stores/userStore'
import { ElMessage } from 'element-plus'
import { CaretBottom } from '@element-plus/icons-vue'

const router = useRouter()
const userStore = useUserStore()

onMounted(() => {
  console.log('AppHeader mounted', userStore.getUsername)
})

const handleCommand = (command: string) => {
  if (command === 'logout') {
    userStore.logout()
    router.push('/login')
    ElMessage.success('已退出登录')
  }
}
</script>

<style scoped lang="scss">
.app-header {
  height: 50px;
  background-color: #fff;
  border-bottom: 1px solid #ebeef5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.04);
  position: relative;
  z-index: 2;

  .header-left {
    h2 {
      margin: 0;
      color: #409eff;
      font-size: 20px;
    }
  }

  .header-right {
    .user-dropdown {
      margin-left: auto;
      cursor: pointer;

      .user-info {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 4px 12px;
        border-radius: 4px;
        transition: background-color 0.3s;

        &:hover {
          background-color: rgba(0, 0, 0, 0.05);
        }

        .username {
          font-size: 14px;
          color: var(--el-text-color-primary);
          max-width: 120px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }

        .dropdown-icon {
          font-size: 12px;
          color: var(--el-text-color-secondary);
        }
      }
    }
  }
}
</style>