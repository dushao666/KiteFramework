<template>
  <div class="role-container">
    <div class="search-bar">
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="关键字">
          <el-input v-model="queryParams.keyword" placeholder="请输入角色名称或编码" clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="请选择状态" clearable>
            <el-option :value="1" label="启用" />
            <el-option :value="0" label="禁用" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">{{ NAMES.BUTTONS.SEARCH }}</el-button>
          <el-button @click="resetQuery">{{ NAMES.BUTTONS.RESET }}</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="table-container">
      <div class="toolbar">
        <el-button type="primary" @click="handleAdd">{{ NAMES.BUTTONS.ADD }}</el-button>
      </div>

      <el-table v-loading="loading" :data="roleList" border>
        <el-table-column type="index" label="#" width="50" align="center" />
        <el-table-column prop="name" label="角色名称" align="center" />
        <el-table-column prop="code" label="角色编码" align="center" />
        <el-table-column prop="description" label="描述" align="center" show-overflow-tooltip />
        <el-table-column prop="status" label="状态" align="center">
          <template #default="scope">
            <el-switch
              v-model="scope.row.status"
              :active-value="1"
              :inactive-value="0"
              @change="handleStatusChange(scope.row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" align="center" />
        <el-table-column label="操作" width="300" align="center">
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
              <el-button type="danger" size="small" @click="handleDelete(scope.row)">
                <el-icon>
                  <Delete />
                </el-icon>
                <span>删除</span>
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryParams.pageNum"
          v-model:page-size="queryParams.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="total"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </div>

    <!-- 添加/编辑角色对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="500px" append-to-body>
      <el-form ref="roleFormRef" :model="roleForm" :rules="rules" label-width="100px">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="角色编码" prop="code">
          <el-input v-model="roleForm.code" placeholder="请输入角色编码" />
        </el-form-item>
        <el-form-item label="角色描述">
          <el-input v-model="roleForm.description" type="textarea" placeholder="请输入角色描述" />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="roleForm.status" :active-value="1" :inactive-value="0" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitForm">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配权限对话框 -->
    <el-dialog title="分配权限" v-model="permissionDialogVisible" width="500px" append-to-body>
      <el-form label-width="100px">
        <el-form-item label="角色名称">
          <span>{{ currentRole?.name }}</span>
        </el-form-item>
        <el-form-item label="菜单权限">
          <el-tree
            ref="menuTreeRef"
            :data="menuOptions"
            show-checkbox
            node-key="id"
            :props="{ label: 'name', children: 'children' }"
            default-expand-all
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="permissionDialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitPermission">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Edit, Delete, Setting } from '@element-plus/icons-vue'
import { NAMES } from '../../../constants'
import { 
  getRoleList, 
  addRole, 
  updateRole, 
  deleteRole, 
  updateRoleStatus, 
  getRoleMenus, 
  assignRoleMenus,
  RoleItem,
  RoleQuery
} from '../../../api/role'
import { getMenuTree } from '../../../api/menu'

// 查询参数
const queryParams = reactive<RoleQuery>({
  keyword: '',
  status: undefined,
  pageNum: 1,
  pageSize: 10
})

// 角色表单
const roleForm = reactive<RoleItem>({
  id: undefined,
  name: '',
  code: '',
  description: '',
  status: 1
})

// 表单校验规则
const rules = {
  name: [{ required: true, message: '角色名称不能为空', trigger: 'blur' }],
  code: [{ required: true, message: '角色编码不能为空', trigger: 'blur' }]
}

const loading = ref(false)
const roleList = ref<RoleItem[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const dialogType = ref('add')
const roleFormRef = ref()
const permissionDialogVisible = ref(false)
const currentRole = ref<RoleItem | null>(null)
const menuOptions = ref<any[]>([])
const menuTreeRef = ref()

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加角色' : '编辑角色'
})

// 获取角色列表
const getList = async () => {
  loading.value = true
  try {
    const res = await getRoleList(queryParams)
    if (res.code === 200) {
      roleList.value = res.data.list
      total.value = res.data.total
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
  queryParams.keyword = ''
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
const handleEdit = (row: RoleItem) => {
  dialogType.value = 'edit'
  resetForm()
  Object.assign(roleForm, row)
  dialogVisible.value = true
}

// 删除角色
const handleDelete = (row: RoleItem) => {
  ElMessageBox.confirm('确定要删除该角色吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      if (!row.id) {
        ElMessage.error('角色ID不存在')
        return
      }

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
  }).catch(() => {})
}

// 处理状态变更
const handleStatusChange = async (row: RoleItem) => {
  try {
    if (!row.id) {
      ElMessage.error('角色ID不存在')
      return
    }

    const res = await updateRoleStatus(row.id, row.status)
    if (res.code === 200) {
      ElMessage.success('状态更新成功')
    } else {
      ElMessage.error(res.message || '状态更新失败')
      // 恢复原状态
      row.status = row.status === 1 ? 0 : 1
    }
  } catch (error) {
    console.error('更新角色状态失败:', error)
    ElMessage.error('状态更新失败')
    // 恢复原状态
    row.status = row.status === 1 ? 0 : 1
  }
}

// 重置表单
const resetForm = () => {
  roleForm.id = undefined
  roleForm.name = ''
  roleForm.code = ''
  roleForm.description = ''
  roleForm.status = 1

  if (roleFormRef.value) {
    roleFormRef.value.resetFields()
  }
}

// 提交表单
const submitForm = async () => {
  if (!roleFormRef.value) return

  await roleFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return

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
    }
  })
}

// 获取菜单树
const getMenuTree = async () => {
  try {
    const res = await getMenuTree()
    if (res.code === 200) {
      menuOptions.value = res.data
    } else {
      ElMessage.error(res.message || '获取菜单树失败')
    }
  } catch (error) {
    console.error('获取菜单树失败:', error)
    ElMessage.error('获取菜单树失败')
  }
}

// 处理权限分配
const handlePermission = async (row: RoleItem) => {
  if (!row.id) {
    ElMessage.error('角色ID不存在')
    return
  }

  currentRole.value = row
  permissionDialogVisible.value = true

  // 获取菜单树
  await getMenuTree()

  // 获取角色已有权限
  try {
    const res = await getRoleMenus(row.id)
    if (res.code === 200) {
      // 设置选中的节点
      if (menuTreeRef.value) {
        menuTreeRef.value.setCheckedKeys(res.data)
      }
    } else {
      ElMessage.error(res.message || '获取角色权限失败')
    }
  } catch (error) {
    console.error('获取角色权限失败:', error)
    ElMessage.error('获取角色权限失败')
  }
}

// 提交权限
const submitPermission = async () => {
  if (!currentRole.value || !currentRole.value.id) {
    ElMessage.error('角色信息不完整')
    return
  }

  if (!menuTreeRef.value) {
    ElMessage.error('菜单树未加载')
    return
  }

  try {
    // 获取选中的节点和半选中的节点
    const checkedKeys = menuTreeRef.value.getCheckedKeys()
    const halfCheckedKeys = menuTreeRef.value.getHalfCheckedKeys()
    const menuIds = [...checkedKeys, ...halfCheckedKeys]

    const res = await assignRoleMenus({
      roleId: currentRole.value.id,
      menuIds
    })

    if (res.code === 200) {
      ElMessage.success('权限分配成功')
      permissionDialogVisible.value = false
    } else {
      ElMessage.error(res.message || '权限分配失败')
    }
  } catch (error) {
    console.error('分配权限失败:', error)
    ElMessage.error('权限分配失败')
  }
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
  }

  .pagination-container {
    margin-top: 15px;
    display: flex;
    justify-content: flex-end;
  }
}

/* 操作按钮样式 */
.operation-buttons {
  display: flex;
  justify-content: center;
  gap: 8px;

  .el-button {
    padding: 4px 8px;

    .el-icon {
      margin-right: 4px;
    }
  }
}
</style> 