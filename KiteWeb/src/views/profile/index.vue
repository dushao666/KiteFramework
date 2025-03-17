<template>
  <div class="profile-container">
    <el-card class="profile-card">
      <template #header>
        <div class="card-header">
          <h3>个人信息</h3>
        </div>
      </template>
      
      <div class="profile-content">
        <div class="avatar-container">
          <el-avatar :size="100" src="../../assets/avatar.png" />
          <h2>{{ username }}</h2>
        </div>
        
        <el-divider />
        
        <el-form label-width="100px" class="profile-form">
          <el-form-item label="用户名">
            <span>{{ username }}</span>
          </el-form-item>
          <el-form-item label="用户ID">
            <span>{{ userid }}</span>
          </el-form-item>
          <el-form-item label="登录时间">
            <span>{{ loginTime }}</span>
          </el-form-item>
        </el-form>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useUserStore } from '../../stores/userStore'

const userStore = useUserStore()
const username = computed(() => userStore.username || '未知用户')
const userid = computed(() => userStore.userid || '未知ID')
const loginTime = ref(new Date().toLocaleString())
</script>

<style scoped lang="scss">
.profile-container {
  padding: 20px;
  
  .profile-card {
    max-width: 600px;
    margin: 0 auto;
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      h3 {
        margin: 0;
        color: #303133;
      }
    }
    
    .profile-content {
      padding: 20px 0;
      
      .avatar-container {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-bottom: 20px;
        
        h2 {
          margin-top: 15px;
          color: #303133;
        }
      }
      
      .profile-form {
        margin-top: 20px;
      }
    }
  }
}
</style> 