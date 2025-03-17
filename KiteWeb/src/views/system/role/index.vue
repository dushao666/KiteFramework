<template>
  <div class="role-container">
    <div class="search-bar">
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="角色名称">
          <el-input v-model="queryParams.name" placeholder="请输入角色名称" clearable />
        </el-form-item>
        <el-form-item label="角色编码">
          <el-input v-model="queryParams.code" placeholder="请输入角色编码" clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="角色状态" clearable style="width: 120px"
            @change="handleQuery">
            <el-option label="正常" :value="0" />
            <el-option label="停用" :value="1" />
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

      <el-table v-loading="loading" :data="roleList" border>
        <el-table-column prop="id" width="80" align="center" label="ID" />
        <el-table-column prop="name" label="角色名称" align="center" min-width="120" />
        <el-table-column prop="code" label="角色编码" align="center" min-width="120" />
        <el-table-column prop="description" label="描述" align="center" show-overflow-tooltip min-width="180" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="scope">
            <el-switch v-model="scope.row.status" :active-value="0" :inactive-value="1"
              @change="handleStatusChange(scope.row)" />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" align="center" width="180" show-overflow-tooltip />
        <el-table-column label="操作" width="220" align="center" fixed="right">
          <template #default="scope">
            <div class="operation-buttons">
              <el-button type="primary" size="small" @click="handleEdit(scope.row)">
                <el-icon>
                  <Edit />
                </el-icon>
                <span>编辑</span>
              </el-button>
              <el-button type="success" size="small" @click="handlePermission(scope.row)">
                <el-icon>
                  <Setting />
                </el-icon>
                <span>权限</span>
              </el-button>
              <el-button type="danger" size="small" @click="handleDelete(scope.row)"
                :disabled="scope.row.code === 'admin'">
                <el-icon>
                  <Delete />
                </el-icon>
                <span>删除</span>
              </el-button>
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

    <!-- 添加/编辑角色对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="500px" append-to-body destroy-on-close>
      <el-form ref="roleFormRef" :model="roleForm" :rules="rules" label-width="100px">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="角色编码" prop="code">
          <el-input v-model="roleForm.code" placeholder="请输入角色编码" :disabled="dialogType === 'edit'" />
        </el-form-item>
        <el-form-item label="角色状态" prop="status">
          <el-radio-group v-model="roleForm.status">
            <el-radio :label="0">正常</el-radio>
            <el-radio :label="1">停用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="角色描述" prop="description">
          <el-input v-model="roleForm.description" type="textarea" :rows="3" placeholder="请输入角色描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitForm" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配权限对话框 -->
    <el-dialog title="分配权限" v-model="permissionDialogVisible" width="600px" append-to-body destroy-on-close>
      <div v-if="currentRole">
        <p class="permission-title">为角色 <strong>{{ currentRole.name }}</strong> 分配权限</p>
        <el-tree ref="permissionTreeRef" :data="permissionTree" show-checkbox node-key="id"
          :props="{ label: 'name', children: 'children' }" default-expand-all v-loading="permissionLoading" />
      </div>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="permissionDialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="savePermissions" :loading="submitLoading">{{ NAMES.BUTTONS.CONFIRM
          }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getRoleList, addRole, updateRole, deleteRole, updateRoleStatus, getRolePermissions, saveRolePermissions } from '../../../api/role'
import { getMenuTree } from '../../../api/menu'
import { NAMES } from '../../../constants'
import { Edit, Delete, Plus, Search, Refresh, Setting } from '@element-plus/icons-vue'

// 查询参数
const queryParams = reactive({
  name: '',
  code: '',
  status: undefined as number | undefined,
  pageNum: 1,
  pageSize: 10
})

// 角色表单
interface RoleFormData {
  id?: string | number;
  name: string;
  code: string;
  description: string;
  status: number;
}

const roleForm = reactive<RoleFormData>({
  id: '',
  name: '',
  code: '',
  description: '',
  status: 0
})

// 表单校验规则
const rules = {
  name: [
    { required: true, message: '角色名称不能为空', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '角色编码不能为空', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ]
}

const loading = ref(false)
const submitLoading = ref(false)
const permissionLoading = ref(false)
const roleList = ref<any[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const dialogType = ref('add')
const roleFormRef = ref()
const permissionDialogVisible = ref(false)
const permissionTreeRef = ref()
const permissionTree = ref<any[]>([])
const currentRole = ref<any>(null)

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加角色' : '编辑角色'
})

// 查询角色列表
const getList = async () => {
  loading.value = true
  try {
    const res = await getRoleList({
      ...queryParams
    })

    if (res.code === 200) {
      roleList.value = res.data
      total.value = res.total || res.data.length
    } else {
      ElMessage.error(res.message || '获取角色列表失败')
    }
  } catch (error) {
    console.error('获取角色列表失败:', error)
    ElMessage.error('获取角色列表失败')
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
  queryParams.code = ''
  queryParams.status = undefined
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

// 添加角色
const handleAdd = () => {
  dialogType.value = 'add'
  resetForm()
  dialogVisible.value = true
}

// 编辑角色
const handleEdit = (row: any) => {
  dialogType.value = 'edit'
  resetForm()
  Object.assign(roleForm, row)
  dialogVisible.value = true
}

// 删除角色
const handleDelete = (row: any) => {
  if (row.code === 'admin') {
    ElMessage.warning('超级管理员角色不能删除')
    return
  }

  ElMessageBox.confirm('确定要删除该角色吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const res = await deleteRole(row.id)
      if (res.code === 200) {
        ElMessage.success('删除成功')
        getList()
      } else {
        ElMessage.error(res.message || '删除失败')
      }
    } catch (error) {
      console.error('删除角色失败:', error)
      ElMessage.error('删除失败')
    }
  }).catch(() => { })
}

// 处理状态变更
const handleStatusChange = async (row: any) => {
  if (row.code === 'admin' && row.status === 1) {
    ElMessage.warning('超级管理员角色不能停用')
    row.status = 0
    return
  }

  const newStatus = row.status // switch 已经改变了状态值
  const statusText = newStatus === 0 ? '启用' : '停用'
  const originalStatus = newStatus === 0 ? 1 : 0 // 保存原始状态用于恢复

  try {
    const res = await updateRoleStatus(row.id, newStatus)
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
    console.error('更新角色状态失败:', error)
    ElMessage.error(`${statusText}失败`)
    // 恢复原状态
    row.status = originalStatus
  }
}

// 处理权限分配
const handlePermission = async (row: any) => {
  currentRole.value = row
  loading.value = true
  permissionLoading.value = true

  try {
    // 获取菜单树
    const menuRes = await getMenuTree()
    if (menuRes.code === 200) {
      permissionTree.value = menuRes.data

      // 获取角色已有权限
      const permRes = await getRolePermissions(row.id)
      if (permRes.code === 200) {
        // 设置选中的节点 - 在显示对话框之前预先加载
        setTimeout(() => {
          permissionDialogVisible.value = true
          // 使用 nextTick 确保树渲染完成后再设置选中状态
          nextTick(() => {
            if (permissionTreeRef.value) {
              permissionTreeRef.value.setCheckedKeys(permRes.data)
              permissionLoading.value = false
            }
          })
        }, 100)
      }
    }
  } catch (error) {
    console.error('获取权限数据失败:', error)
    ElMessage.error('获取权限数据失败')
    permissionLoading.value = false
  } finally {
    loading.value = false
  }
}

// 保存权限设置
const savePermissions = async () => {
  if (!currentRole.value || !permissionTreeRef.value) return

  submitLoading.value = true
  try {
    // 获取选中的节点ID
    const checkedKeys = permissionTreeRef.value.getCheckedKeys()
    const halfCheckedKeys = permissionTreeRef.value.getHalfCheckedKeys()
    const allKeys = [...checkedKeys, ...halfCheckedKeys]

    const res = await saveRolePermissions(currentRole.value.id, allKeys)
    if (res.code === 200) {
      ElMessage.success('权限分配成功')
      permissionDialogVisible.value = false
    } else {
      ElMessage.error(res.message || '权限分配失败')
    }
  } catch (error) {
    console.error('保存权限失败:', error)
    ElMessage.error('权限分配失败')
  } finally {
    submitLoading.value = false
  }
}

// 重置表单
const resetForm = () => {
  roleForm.id = ''
  roleForm.name = ''
  roleForm.code = ''
  roleForm.description = ''
  roleForm.status = 0

  if (roleFormRef.value) {
    roleFormRef.value.resetFields()
  }
}

// 提交表单
const submitForm = async () => {
  if (!roleFormRef.value) return

  await roleFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return

    submitLoading.value = true
    try {
      const api = dialogType.value === 'add' ? addRole : updateRole
      const res = await api(roleForm)

      if (res.code === 200) {
        ElMessage.success(dialogType.value === 'add' ? '添加成功' : '更新成功')
        dialogVisible.value = false
        getList()
      } else {
        ElMessage.error(res.message || (dialogType.value === 'add' ? '添加失败' : '更新失败'))
      }
    } catch (error) {
      console.error(dialogType.value === 'add' ? '添加角色失败:' : '更新角色失败:', error)
      ElMessage.error(dialogType.value === 'add' ? '添加失败' : '更新失败')
    } finally {
      submitLoading.value = false
    }
  })
}

onMounted(() => {
  getList()
})
</script>

<style scoped lang="scss">
.role-container {
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
  gap: 8px;
  flex-wrap: nowrap;

  .el-button {
    padding: 4px 8px;
    margin: 2px 0;

    .el-icon {
      margin-right: 4px;
    }
  }
}

.permission-title {
  margin-bottom: 15px;
  font-size: 16px;
  color: #606266;
}
</style>