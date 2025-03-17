<template>
  <div class="not-found">
    <div class="content">
      <h1>404</h1>
      <h2>页面不存在</h2>
      <p>抱歉，您访问的页面不存在或已被删除</p>
      <el-button type="primary" @click="goHome">返回首页</el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goHome = () => {
  router.push('/')
}
</script>

<style scoped lang="scss">
.not-found {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  min-height: 500px;
  text-align: center;
  
  .content {
    h1 {
      font-size: 120px;
      color: #409eff;
      margin: 0;
      line-height: 1.2;
    }
    
    h2 {
      font-size: 30px;
      color: #303133;
      margin: 20px 0;
    }
    
    p {
      font-size: 16px;
      color: #606266;
      margin-bottom: 30px;
    }
  }
}
</style> 