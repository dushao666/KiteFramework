<template>
  <div class="login-container">
    <el-form :model="formData" ref="loginFormRef">
      <p class="form-title">{{ NAMES.APP_NAME }}</p>

      <el-form-item>
        <el-input v-model="formData.Login" placeholder="请输入用户名" prefix-icon="User" />
      </el-form-item>

      <el-form-item>
        <el-input v-model="formData.Password" type="password" placeholder="请输入密码" prefix-icon="Lock"
          @keyup.enter="handleLogin" />
      </el-form-item>

      <el-button class="submit" type="primary" :loading="loading" @click="handleLogin">
        登录
      </el-button>

      <!-- <el-button class="submit" type="primary" :loading="loading" @click="handleDingtalkLogin" v-if="isDingTalk">
        钉钉一键登录
      </el-button> -->

      <p class="signup-link">
        没有账号?
        <a href="#">注册</a>
      </p>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import * as dd from 'dingtalk-jsapi'
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance } from 'element-plus'
import { useUserStore } from '../../stores/userStore'
import { login } from '../../api/user'
import { NAMES } from '../../constants/index'

const router = useRouter()
const userStore = useUserStore()
const loginFormRef = ref<FormInstance>()
const loading = ref(false)
const isDingTalk = ref(false)

const formData = reactive({
  Login: '',
  Password: '',
  Type: 1,
  DingTalkAuthCode: '',
})

// 快捷键填充账号密码
const handleShortcut = (e: KeyboardEvent) => {
  if (e.ctrlKey && e.altKey && e.key.toLowerCase() === 'l') {
    formData.Login = 'admin'
    formData.Password = '123456'
    handleLogin()
  }
}

// 判断是否为钉钉环境
onMounted(() => {
  // 添加快捷键监听
  window.addEventListener('keydown', handleShortcut)

  if (dd.env.platform !== 'notInDingTalk') {
    isDingTalk.value = true
    console.log('当前是钉钉环境')
    formData.Type = 2
    handleDingtalkLogin()
  } else {
    console.log('当前不是钉钉环境')
    formData.Type = 1
  }
})

// 组件卸载时移除事件监听
onUnmounted(() => {
  window.removeEventListener('keydown', handleShortcut)
})

const handleDingtalkLogin = () => {
  if (dd && typeof dd.requestAuthCode === 'function') {
    loading.value = true
    dd.requestAuthCode({
      corpId: import.meta.env.VITE_CORP_ID,
      clientId: import.meta.env.VITE_CLIENT_ID,
      onSuccess: function (result: any) {
        formData.DingTalkAuthCode = result.code
        console.log('获取授权码成功result.code:', result.code)
        handleTokenLogin()
      },
      onFail: function (err: any) {
        console.error('钉钉授权失败:', err)
        ElMessage.error('钉钉授权失败')
        loading.value = false
      },
    })
  } else {
    console.error('钉钉 SDK 未正确加载')
    ElMessage.error('当前环境不支持钉钉登录')
  }
}

const handleTokenLogin = async () => {
  try {
    const res = await login(formData)
    console.log('登录响应:', res)

    if (res.code === 200) {
      await userStore.setUserInfo({
        username: formData.Login,
        userid: formData.Login,
        userInfo: null,
        accessToken: res.data.toString(),
        refreshToken: ''
      })
      ElMessage.success('登录成功')
      router.push('/home')
    } else {
      ElMessage.error(res.message || '登录失败')
    }
  } catch (error: any) {
    console.error('登录错误:', error)
    ElMessage.error(error.message || '登录失败')
  } finally {
    loading.value = false
  }
}

const handleLogin = async () => {
  loading.value = true
  formData.Type = 1
  handleTokenLogin()
}
</script>

<style scoped lang="scss">
.login-container {
  max-width: 400px;
  margin: 12% auto;
  padding: 20px;
  border: 1px solid #dcdfe6;
  border-radius: 8px;
  background-color: #fff;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);

  .form-title {
    font-size: 1.25rem;
    line-height: 1.75rem;
    font-weight: 600;
    text-align: center;
    color: #303133;
    margin-bottom: 20px;
  }

  .submit {
    display: block;
    width: 100%;
    margin-top: 20px;
  }

  .signup-link {
    margin-top: 15px;
    color: #606266;
    font-size: 0.875rem;
    text-align: center;

    a {
      color: #409eff;
      text-decoration: none;

      &:hover {
        text-decoration: underline;
      }
    }
  }
}
</style>