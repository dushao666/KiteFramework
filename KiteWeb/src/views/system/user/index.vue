<template>
  <div class="user-container">
    <div class="search-bar">
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="用户名">
          <el-input v-model="queryParams.name" placeholder="请输入用户名" clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="用户状态" clearable style="width: 120px"
            @change="handleQuery">
            <el-option label="正常" value="0" />
            <el-option label="停用" value="1" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">
            <el-icon>
              <Search />
            </el-icon>
            <span>{{ NAMES.BUTTONS.SEARCH }}</span>
          </el-button>
          <el-button @click="resetQuery">
            <el-icon>
              <Refresh />
            </el-icon>
            <span>{{ NAMES.BUTTONS.RESET }}</span>
          </el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="table-container">
      <div class="toolbar">
        <el-button type="primary" @click="handleAdd">
          <el-icon>
            <Plus />
          </el-icon>
          <span>{{ NAMES.BUTTONS.ADD }}</span>
        </el-button>
      </div>

      <el-table v-loading="loading" :data="userList" border :row-style="{height: '50px'}">
        <el-table-column prop="id" width="80" align="center" label="ID" />
        <el-table-column prop="name" label="用户名" align="center" min-width="120" />
        <el-table-column prop="nickName" label="昵称" align="center" min-width="120" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="scope">
            <el-switch v-model="scope.row.status" :active-value="0" :inactive-value="1"
              @change="handleStatusChange(scope.row)" />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" align="center" width="180" show-overflow-tooltip />
        <el-table-column label="操作" width="300" align="center" fixed="right">
          <template #default="scope">
            <div class="operation-buttons">
              <el-button type="primary" size="small" @click="handleEdit(scope.row)">
                <el-icon>
                  <Edit />
                </el-icon>
                <span>编辑</span>
              </el-button>
              <el-button type="success" size="small" @click="handleRole(scope.row)">
                <el-icon>
                  <UserFilled />
                </el-icon>
                <span>角色</span>
              </el-button>
              <el-button type="info" size="small" @click="handlePost(scope.row)">
                <el-icon>
                  <Position />
                </el-icon>
                <span>岗位</span>
              </el-button>
              
              <!-- 更多操作下拉菜单 -->
              <el-dropdown trigger="click">
                <el-button size="small" type="default">
                  <el-icon>
                    <More />
                  </el-icon>
                  <span class="more-text">更多</span>
                </el-button>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item @click="handleResetPwd(scope.row)">
                      <el-icon><Key /></el-icon>
                      <span>重置密码</span>
                    </el-dropdown-item>
                    <el-dropdown-item @click="handleDelete(scope.row)" 
                      :disabled="scope.row.name === 'admin'" 
                      :class="{ 'disabled-dropdown-item': scope.row.name === 'admin' }">
                      <el-icon><Delete /></el-icon>
                      <span>删除</span>
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </div>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container" v-if="total > 0">
        <el-pagination v-model:current-page="queryParams.pageNum" v-model:page-size="queryParams.pageSize"
          :page-sizes="[10, 20, 50, 100]" layout="total, sizes, prev, pager, next, jumper" :total="total"
          @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </div>

    <!-- 添加/编辑用户对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="500px" append-to-body destroy-on-close>
      <el-form ref="userFormRef" :model="userForm" :rules="rules" label-width="100px">
        <el-form-item label="用户名" prop="name">
          <el-input v-model="userForm.name" placeholder="请输入用户名" :disabled="dialogType === 'edit'" />
        </el-form-item>
        <el-form-item label="昵称" prop="nickName">
          <el-input v-model="userForm.nickName" placeholder="请输入昵称" />
        </el-form-item>
        <el-form-item label="密码" prop="passWord" v-if="dialogType === 'add'">
          <el-input v-model="userForm.passWord" placeholder="请输入密码" type="password" show-password />
        </el-form-item>
        <el-form-item label="用户状态" prop="status">
          <el-radio-group v-model="userForm.status">
            <el-radio :label="0">正常</el-radio>
            <el-radio :label="1">停用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="所属角色" prop="roleIds">
          <el-select v-model="userForm.roleIds" multiple placeholder="请选择角色" style="width: 100%">
            <el-option
              v-for="item in roleOptions"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="所属岗位" prop="postIds">
          <el-select v-model="userForm.postIds" multiple placeholder="请选择岗位" style="width: 100%">
            <el-option
              v-for="item in postOptions"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitForm" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 重置密码对话框 -->
    <el-dialog title="重置密码" v-model="resetPwdVisible" width="500px" append-to-body destroy-on-close>
      <el-form ref="resetPwdFormRef" :model="resetPwdForm" :rules="resetPwdRules" label-width="100px">
        <el-form-item label="新密码" prop="newPassword">
          <el-input v-model="resetPwdForm.newPassword" placeholder="请输入新密码" type="password" show-password />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input v-model="resetPwdForm.confirmPassword" placeholder="请确认新密码" type="password" show-password />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="resetPwdVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitResetPwd" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配角色对话框 -->
    <el-dialog title="分配角色" v-model="roleDialogVisible" width="500px" append-to-body destroy-on-close>
      <div v-if="currentUser">
        <p class="role-title">为用户 <strong>{{ currentUser.name }}</strong> 分配角色</p>
        <el-transfer
          v-model="selectedRoleIds"
          :data="roleOptions"
          :titles="['可选角色', '已选角色']"
          :props="{
            key: 'id',
            label: 'name'
          }"
          v-loading="roleLoading"
          class="custom-transfer"
        ></el-transfer>
      </div>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="roleDialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="saveUserRoles" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配岗位对话框 -->
    <el-dialog title="分配岗位" v-model="postDialogVisible" width="500px" append-to-body destroy-on-close>
      <div v-if="currentUser">
        <p class="role-title">为用户 <strong>{{ currentUser.name }}</strong> 分配岗位</p>
        <el-transfer
          v-model="selectedPostIds"
          :data="postOptions"
          :titles="['可选岗位', '已选岗位']"
          :props="{
            key: 'id',
            label: 'name'
          }"
          v-loading="postLoading"
          class="custom-transfer"
        ></el-transfer>
      </div>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="postDialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="saveUserPosts" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  getUserList, 
  addUser, 
  updateUser, 
  deleteUser, 
  updateUserStatus, 
  resetUserPassword, 
  getUserRoles, 
  assignUserRoles, 
  UserItem, 
  getUserPosts, 
  getUserDetail, 
  isEditingUser,
  assignUserPosts 
} from '../../../api/user'
import { getRoleList } from '../../../api/role'
import { getPostList, PostItem } from '../../../api/post'
import { NAMES } from '../../../constants'
import { Edit, Delete, Plus, Search, Refresh, Key, UserFilled, Position, More } from '@element-plus/icons-vue'

// 查询参数
const queryParams = reactive({
  name: '',
  status: '',
  pageNum: 1,
  pageSize: 10
})

// 用户表单
interface UserFormData extends UserItem {
  passWord?: string;
}

const userForm = reactive<UserFormData>({
  id: '',
  name: '',
  nickName: '',
  status: 0,
  passWord: '',
  roleIds: [],
  postIds: []
})

// 重置密码表单
interface ResetPwdFormData {
  userId: string | number;
  newPassword: string;
  confirmPassword: string;
}

const resetPwdForm = reactive<ResetPwdFormData>({
  userId: '',
  newPassword: '',
  confirmPassword: ''
})

// 表单校验规则
const rules = {
  name: [
    { required: true, message: '用户名不能为空', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  nickName: [
    { required: true, message: '昵称不能为空', trigger: 'blur' }
  ],
  passWord: [
    { required: true, message: '密码不能为空', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ]
}

// 重置密码校验规则
const resetPwdRules = {
  newPassword: [
    { required: true, message: '新密码不能为空', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '确认密码不能为空', trigger: 'blur' },
    {
      validator: (rule: any, value: string, callback: Function) => {
        if (value !== resetPwdForm.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

const loading = ref(false)
const submitLoading = ref(false)
const roleLoading = ref(false)
const postLoading = ref(false)
const userList = ref<UserItem[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const dialogType = ref('add')
const userFormRef = ref()
const resetPwdVisible = ref(false)
const resetPwdFormRef = ref()
const roleDialogVisible = ref(false)
const postDialogVisible = ref(false)
const currentUser = ref<UserItem | null>(null)
const roleOptions = ref<any[]>([])
const postOptions = ref<any[]>([])
const selectedRoleIds = ref<number[]>([])
const selectedPostIds = ref<number[]>([])

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加用户' : '编辑用户'
})

// 查询用户列表
const getList = async () => {
  loading.value = true
  try {
    const res = await getUserList({
      ...queryParams
    })

    if (res.code === 200) {
      // 确保status值是数字类型
      userList.value = res.data.map(user => ({
        ...user,
        status: typeof user.status === 'string' ? parseInt(user.status) : user.status
      }))
      total.value = res.total || res.data.length
    } else {
      ElMessage.error(res.message || '获取用户列表失败')
    }
  } catch (error) {
    console.error('获取用户列表出错：', error)
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 查询按钮点击事件
const handleQuery = () => {
  queryParams.pageNum = 1
  getList()
}

// 重置查询条件
const resetQuery = () => {
  queryParams.name = ''
  queryParams.status = ''
  queryParams.pageNum = 1
  getList()
}

// 处理页码变化
const handleCurrentChange = (val: number) => {
  queryParams.pageNum = val
  getList()
}

// 处理每页条数变化
const handleSizeChange = (val: number) => {
  queryParams.pageSize = val
  queryParams.pageNum = 1
  getList()
}

// 加载角色列表
const loadRoleOptions = async () => {
  try {
    const res = await getRoleList()
    if (res.code === 200) {
      roleOptions.value = res.data.map(role => ({
        id: role.id,
        name: role.name
      }))
    } else {
      ElMessage.warning('获取角色列表失败')
    }
  } catch (error) {
    ElMessage.error('获取角色数据失败')
  }
}

// 加载岗位列表
const loadPostOptions = async () => {
  try {
    postLoading.value = true
    const res = await getPostList()
    if (res.code === 200) {
      postOptions.value = res.data.map(post => ({
        id: post.id,
        name: post.name
      }))
    } else {
      ElMessage.warning('获取岗位列表失败')
    }
  } catch (error) {
    ElMessage.error('获取岗位数据失败')
  } finally {
    postLoading.value = false
  }
}

// 添加用户
const handleAdd = () => {
  dialogType.value = 'add'
  resetForm()
  loadRoleOptions()
  loadPostOptions()
  dialogVisible.value = true
}

// 编辑用户
const handleEdit = async (row: UserItem) => {
  // 设置标记，表示正在编辑用户
  isEditingUser.value = true;
  
  dialogType.value = 'edit'
  resetForm()
  
  try {
    // 获取用户详情
    const detailRes = await getUserDetail(row.id);
    
    if (detailRes.code === 200) {
      // 手动设置用户表单数据，防止自动触发其他API调用
      userForm.id = detailRes.data.id;
      userForm.name = detailRes.data.name;
      userForm.nickName = detailRes.data.nickName;
      // 确保status是数字
      userForm.status = typeof detailRes.data.status === 'string' 
        ? parseInt(detailRes.data.status) 
        : detailRes.data.status;
      
      // 手动设置角色和岗位数据
      if (detailRes.data.roleIds) {
        userForm.roleIds = [...detailRes.data.roleIds];
      } else {
        userForm.roleIds = [];
      }
      
      if (detailRes.data.postIds) {
        userForm.postIds = [...detailRes.data.postIds];
      } else {
        userForm.postIds = [];
      }
      
      // 单独加载角色和岗位列表
      await Promise.all([loadRoleOptions(), loadPostOptions()]);
      
      dialogVisible.value = true
    } else {
      ElMessage.error(detailRes.message || '获取用户详情失败')
    }
  } catch (error) {
    console.error('编辑用户出错:', error)
    ElMessage.error('获取用户详情失败')
  } finally {
    // 清除标记
    setTimeout(() => {
      isEditingUser.value = false;
    }, 500);
  }
}

// 删除用户
const handleDelete = (row: UserItem) => {
  if (row.name === 'admin') {
    ElMessage.warning('超级管理员用户不能删除')
    return
  }

  ElMessageBox.confirm('确定要删除该用户吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const res = await deleteUser(row.id)
      if (res.code === 200) {
        ElMessage.success('删除成功')
        getList()
      } else {
        ElMessage.error(res.message || '删除失败')
      }
    } catch (error) {
      ElMessage.error('删除用户失败')
    }
  }).catch(() => { })
}

// 处理状态变更
const handleStatusChange = async (row: UserItem) => {
  // 确保状态是数字类型
  const status = typeof row.status === 'string' ? parseInt(row.status) : row.status;
  
  if (row.name === 'admin' && status === 1) {
    ElMessage.warning('超级管理员用户不能停用')
    row.status = 0
    return
  }

  const newStatus = status; // 保存新状态值
  const statusText = newStatus === 0 ? '启用' : '停用'
  const originalStatus = newStatus === 0 ? 1 : 0 // 保存原始状态用于恢复

  try {
    const res = await updateUserStatus(row.id, newStatus)
    if (res.code === 200) {
      ElMessage.success(`${statusText}成功`)
      // 刷新列表
      getList()
    } else {
      ElMessage.error(res.message || `${statusText}失败`)
      // 恢复原状态
      row.status = originalStatus
    }
  } catch (error) {
    console.error('更新用户状态失败:', error)
    ElMessage.error('更新用户状态失败')
    // 恢复原状态
    row.status = originalStatus
  }
}

// 处理重置密码
const handleResetPwd = (row: UserItem) => {
  resetPwdForm.userId = row.id
  resetPwdForm.newPassword = ''
  resetPwdForm.confirmPassword = ''
  resetPwdVisible.value = true
  
  if (resetPwdFormRef.value) {
    resetPwdFormRef.value.resetFields()
  }
}

// 提交重置密码
const submitResetPwd = async () => {
  if (!resetPwdFormRef.value) return

  await resetPwdFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return

    try {
      submitLoading.value = true
      const res = await resetUserPassword({
        userId: resetPwdForm.userId,
        newPassword: resetPwdForm.newPassword
      })

      if (res.code === 200) {
        ElMessage.success('密码重置成功')
        resetPwdVisible.value = false
      } else {
        ElMessage.error(res.message || '密码重置失败')
      }
    } catch (error) {
      ElMessage.error('密码重置失败')
    } finally {
      submitLoading.value = false
    }
  })
}

// 处理角色分配
const handleRole = async (row: UserItem) => {
  currentUser.value = row
  roleLoading.value = true
  roleDialogVisible.value = true
  
  try {
    // 获取所有角色
    const roleRes = await getRoleList()
    if (roleRes.code === 200) {
      roleOptions.value = roleRes.data.map(role => ({
        id: role.id,
        name: role.name,
        disabled: false
      }))
      
      // 获取用户已有角色
      const userRoleRes = await getUserRoles(row.id)
      if (userRoleRes.code === 200) {
        selectedRoleIds.value = userRoleRes.data
      } else {
        selectedRoleIds.value = []
        ElMessage.warning('获取用户角色失败')
      }
    } else {
      ElMessage.error(roleRes.message || '获取角色列表失败')
    }
  } catch (error) {
    ElMessage.error('获取角色数据失败')
  } finally {
    roleLoading.value = false
  }
}

// 保存用户角色
const saveUserRoles = async () => {
  if (!currentUser.value) return

  try {
    submitLoading.value = true
    const res = await assignUserRoles(currentUser.value.id, selectedRoleIds.value)
    
    if (res.code === 200) {
      ElMessage.success('角色分配成功')
      roleDialogVisible.value = false
    } else {
      ElMessage.error(res.message || '角色分配失败')
    }
  } catch (error) {
    ElMessage.error('角色分配失败')
  } finally {
    submitLoading.value = false
  }
}

// 处理岗位分配
const handlePost = async (row: UserItem) => {
  currentUser.value = row
  postLoading.value = true
  postDialogVisible.value = true
  
  try {
    // 获取所有岗位
    const postRes = await getPostList()
    if (postRes.code === 200) {
      postOptions.value = postRes.data.map(post => ({
        id: post.id,
        name: post.name,
        disabled: false
      }))
      
      // 获取用户已有岗位
      const userPostRes = await getUserPosts(row.id)
      if (userPostRes.code === 200) {
        selectedPostIds.value = userPostRes.data
      } else {
        selectedPostIds.value = []
        ElMessage.warning('获取用户岗位失败')
      }
    } else {
      ElMessage.error(postRes.message || '获取岗位列表失败')
    }
  } catch (error) {
    ElMessage.error('获取岗位数据失败')
  } finally {
    postLoading.value = false
  }
}

// 保存用户岗位
const saveUserPosts = async () => {
  if (!currentUser.value) return

  try {
    submitLoading.value = true
    const res = await assignUserPosts(currentUser.value.id, selectedPostIds.value)
    
    if (res.code === 200) {
      ElMessage.success('岗位分配成功')
      postDialogVisible.value = false
    } else {
      ElMessage.error(res.message || '岗位分配失败')
    }
  } catch (error) {
    ElMessage.error('岗位分配失败')
  } finally {
    submitLoading.value = false
  }
}

// 提交表单
const submitForm = async () => {
  console.log('开始提交表单，表单引用:', userFormRef.value)
  if (!userFormRef.value) {
    console.error('表单引用不存在')
    return
  }

  try {
    // 表单验证
    const valid = await userFormRef.value.validate()
    console.log('表单验证结果:', valid)
    
    if (!valid) {
      console.log('表单验证失败')
      return
    }
    
    submitLoading.value = true
    
    // 准备提交的数据，确保与后端字段类型匹配
    const submitData = {
      ...userForm,
      // 确保状态为字符串类型
      status: userForm.status.toString(),
      // 确保角色和岗位ID为数组
      roleIds: Array.isArray(userForm.roleIds) ? userForm.roleIds : [],
      postIds: Array.isArray(userForm.postIds) ? userForm.postIds : []
    }
    
    console.log('提交用户表单数据：', JSON.stringify(submitData))
    
    const api = dialogType.value === 'add' ? addUser : updateUser
    const res = await api(submitData)
    console.log('提交用户表单响应：', res)

    if (res.code === 200) {
      ElMessage.success(dialogType.value === 'add' ? '添加成功' : '更新成功')
      dialogVisible.value = false
      getList()
    } else {
      ElMessage.error(res.message || (dialogType.value === 'add' ? '添加失败' : '更新失败'))
    }
  } catch (error) {
    console.error('提交用户表单出错：', error)
    ElMessage.error(dialogType.value === 'add' ? '添加用户失败' : '更新用户失败')
  } finally {
    submitLoading.value = false
  }
}

// 重置表单
const resetForm = () => {
  userForm.id = ''
  userForm.name = ''
  userForm.nickName = ''
  userForm.status = 0
  userForm.passWord = ''
  userForm.roleIds = []
  userForm.postIds = []

  if (userFormRef.value) {
    userFormRef.value.resetFields()
  }
}

onMounted(async () => {
  try {
    console.log('组件挂载，开始初始化')
    // 先加载用户列表
    await getList()
    console.log('用户列表加载完成')
    
    // 加载角色和岗位选项
    await Promise.all([
      loadRoleOptions(),
      loadPostOptions()
    ])
    console.log('角色和岗位选项加载完成')
  } catch (error) {
    console.error('组件初始化出错:', error)
    ElMessage.error('页面初始化失败，请刷新重试')
  }
})
</script>

<style scoped lang="scss">
.user-container {
  padding: 15px;
  background-color: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  height: calc(100vh - 100px);
  display: flex;
  flex-direction: column;
  overflow: hidden;

  .search-bar {
    margin-bottom: 15px;
    flex-shrink: 0;
    background-color: #f8f9fa;
    padding: 15px;
    border-radius: 4px;
  }

  .toolbar {
    margin-bottom: 15px;
    flex-shrink: 0;
    display: flex;
    gap: 10px;
  }

  .table-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    min-height: 0;

    :deep(.el-table) {
      flex: 1;
      overflow: hidden;

      .el-table__header th {
        background-color: #f5f7fa;
        color: #606266;
        font-weight: bold;
      }

      .el-table__row {
        transition: background-color 0.3s;

        &:hover {
          background-color: #f0f9eb;
        }
      }
      
      /* 调整表格单元格内边距 */
      .el-table__cell {
        padding: 8px 0;
      }
    }
  }

  .pagination-container {
    margin-top: 15px;
    display: flex;
    justify-content: flex-end;
    padding: 10px 0;
    background-color: #fff;
  }
}

/* 操作按钮样式 */
.operation-buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
  flex-wrap: nowrap;
  padding: 0 8px;

  .el-button {
    padding: 4px 8px;
    margin: 2px 0;

    .el-icon {
      margin-right: 4px;
    }
  }
  
  .more-text {
    margin-left: 2px;
  }
  
  /* 下拉菜单样式 */
  :deep(.el-dropdown) {
    margin: 2px 0;
    
    .el-dropdown-menu__item {
      display: flex;
      align-items: center;
      padding: 5px 12px;
      
      .el-icon {
        margin-right: 5px;
      }
    }
  }
}

.role-title {
  margin-bottom: 15px;
  font-size: 16px;
  color: #606266;
  font-weight: 500;
}

/* 禁用的下拉菜单项样式 */
.disabled-dropdown-item {
  opacity: 0.6;
  cursor: not-allowed !important;
}

:deep(.el-transfer) {
  display: flex;
  justify-content: center;
  margin: 15px 0;
}

:deep(.custom-transfer) {
  .el-transfer-panel {
    width: 45%;
    border: 1px solid #dcdfe6;
    border-radius: 4px;
    overflow: hidden;
    background: #fff;
    
    .el-transfer-panel__header {
      background-color: #f5f7fa;
      padding: 8px 15px;
      border-bottom: 1px solid #ebeef5;
    }
    
    .el-transfer-panel__body {
      padding-top: 10px;
    }
    
    .el-checkbox__label {
      font-size: 14px;
    }
  }
  
  .el-transfer__buttons {
    padding: 0 10px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    
    .el-button {
      margin: 5px 0;
    }
  }
}
</style> 