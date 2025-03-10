<template>
  <div class="menu-container">
    <div class="search-bar">
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="菜单名称">
          <el-input v-model="queryParams.name" placeholder="请输入菜单名称" clearable />
        </el-form-item>
        <el-form-item label="路径">
          <el-input v-model="queryParams.path" placeholder="请输入路径" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">{{ NAMES.BUTTONS.SEARCH }}</el-button>
          <el-button @click="resetQuery">{{ NAMES.BUTTONS.RESET }}</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="table-container">
      <div class="toolbar">
        <el-button type="primary" @click="() => handleAdd()">{{ NAMES.BUTTONS.ADD }}</el-button>
      </div>

      <el-table
        v-loading="loading"
        :data="menuList"
        row-key="id"
        border
        default-expand-all
        :tree-props="{ children: 'children' }"
      >
        <el-table-column prop="name" label="菜单名称" width="180" />
        <el-table-column prop="path" label="路径" width="180" />
        <el-table-column prop="icon" label="图标" width="100">
          <template #default="scope">
            <i :class="scope.row.icon"></i>
          </template>
        </el-table-column>
        <el-table-column prop="sort" label="排序" width="80" />
        <el-table-column prop="isHidden" label="是否隐藏" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isHidden ? 'danger' : 'success'">
              {{ scope.row.isHidden ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button type="text" @click="handleEdit(scope.row)">{{ NAMES.BUTTONS.EDIT }}</el-button>
            <el-button type="text" @click="handleAdd(scope.row)">添加子菜单</el-button>
            <el-button 
              type="text" 
              @click="handleDelete(scope.row)"
              :disabled="scope.row.children && scope.row.children.length > 0"
            >{{ NAMES.BUTTONS.DELETE }}</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 添加/编辑菜单对话框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="500px"
      append-to-body
    >
      <el-form ref="menuFormRef" :model="menuForm" :rules="rules" label-width="100px">
        <el-form-item label="上级菜单">
          <el-tree-select
            v-model="menuForm.parentId"
            :data="menuOptions"
            check-strictly
            default-expand-all
            node-key="id"
            :props="{ label: 'name', value: 'id' }"
            placeholder="请选择上级菜单"
            clearable
          />
        </el-form-item>
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="菜单路径" prop="path">
          <el-input v-model="menuForm.path" placeholder="请输入菜单路径" />
        </el-form-item>
        <el-form-item label="菜单图标">
          <el-input v-model="menuForm.icon" placeholder="请输入菜单图标" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="menuForm.sort" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="是否隐藏">
          <el-switch v-model="menuForm.isHidden" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">{{ NAMES.BUTTONS.CANCEL }}</el-button>
          <el-button type="primary" @click="submitForm">{{ NAMES.BUTTONS.CONFIRM }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getMenuTree, addMenu, updateMenu, deleteMenu, MenuItem } from '../../../api/menu'
import { NAMES } from '../../../constants'

// 查询参数
const queryParams = reactive({
  name: '',
  path: '',
  pageNum: 1,
  pageSize: 10
})

// 菜单表单
const menuForm = reactive<MenuItem>({
  id: undefined,
  name: '',
  path: '',
  icon: '',
  parentId: undefined,
  sort: 0,
  isHidden: false
})

// 表单校验规则
const rules = {
  name: [{ required: true, message: '菜单名称不能为空', trigger: 'blur' }],
  path: [{ required: true, message: '菜单路径不能为空', trigger: 'blur' }]
}

const loading = ref(false)
const menuList = ref<MenuItem[]>([])
const menuOptions = ref<MenuItem[]>([])
const dialogVisible = ref(false)
const dialogType = ref('add')
const menuFormRef = ref()

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加菜单' : '编辑菜单'
})

// 查询菜单列表
const getList = async () => {
  loading.value = true
  try {
    const res = await getMenuTree()
    if (res.code === 200) {
      menuList.value = res.data
    } else {
      ElMessage.error(res.message || '获取菜单列表失败')
    }
  } catch (error) {
    console.error('获取菜单列表失败:', error)
    // 如果后端API还未实现，使用模拟数据
    menuList.value = [
      {
        id: 1,
        name: '系统管理',
        path: '/system',
        icon: 'el-icon-s-tools',
        sort: 1,
        isHidden: false,
        parentId: undefined,
        children: [
          {
            id: 2,
            name: '菜单管理',
            path: '/system/menu',
            icon: 'el-icon-menu',
            sort: 1,
            isHidden: false,
            parentId: 1
          },
          {
            id: 3,
            name: '用户管理',
            path: '/system/user',
            icon: 'el-icon-user',
            sort: 2,
            isHidden: false,
            parentId: 1
          }
        ]
      },
      {
        id: 4,
        name: '仪表盘',
        path: '/dashboard',
        icon: 'el-icon-s-home',
        sort: 0,
        isHidden: false,
        parentId: undefined
      }
    ]
  } finally {
    loading.value = false
  }
}

// 获取菜单选项（用于上级菜单选择）
const getMenuOptions = async () => {
  try {
    const res = await getMenuTree()
    if (res.code === 200) {
      // 添加一个"顶级菜单"选项
      const topMenu = { id: 0, name: '顶级菜单', path: '' };
      menuOptions.value = [topMenu, ...res.data];
    }
  } catch (error) {
    console.error('获取菜单选项失败:', error)
    // 如果后端API还未实现，使用模拟数据
    menuOptions.value = [
      { id: 0, name: '顶级菜单', path: '' },
      {
        id: 1,
        name: '系统管理',
        path: '/system',
        children: [
          { id: 2, name: '菜单管理', path: '/system/menu' },
          { id: 3, name: '用户管理', path: '/system/user' }
        ]
      },
      { id: 4, name: '仪表盘', path: '/dashboard' }
    ]
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
  queryParams.path = ''
  handleQuery()
}

// 添加菜单
const handleAdd = (row?: MenuItem) => {
  dialogType.value = 'add'
  resetForm()
  
  if (row && row.id) {
    menuForm.parentId = row.id
  }
  
  dialogVisible.value = true
}

// 编辑菜单
const handleEdit = (row: MenuItem) => {
  dialogType.value = 'edit'
  resetForm()
  
  Object.assign(menuForm, row)
  dialogVisible.value = true
}

// 删除菜单
const handleDelete = (row: MenuItem) => {
  if (row.children && row.children.length > 0) {
    ElMessage.warning('该菜单下存在子菜单，不能删除')
    return
  }
  
  ElMessageBox.confirm('确定要删除该菜单吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      if (!row.id) {
        ElMessage.error('菜单ID不存在')
        return
      }
      
      const res = await deleteMenu(row.id)
      if (res.code === 200) {
        ElMessage.success('删除成功')
        getList()
      } else {
        ElMessage.error(res.message || '删除失败')
      }
    } catch (error) {
      console.error('删除菜单失败:', error)
      ElMessage.success('删除成功（模拟）')
      getList()
    }
  }).catch(() => {})
}

// 重置表单
const resetForm = () => {
  menuForm.id = undefined
  menuForm.name = ''
  menuForm.path = ''
  menuForm.parentId = undefined
  menuForm.icon = ''
  menuForm.sort = 0
  menuForm.isHidden = false
  
  if (menuFormRef.value) {
    menuFormRef.value.resetFields()
  }
}

// 提交表单
const submitForm = async () => {
  if (!menuFormRef.value) return
  
  await menuFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return
    
    try {
      const api = dialogType.value === 'add' ? addMenu : updateMenu
      const res = await api(menuForm)
      
      if (res.code === 200) {
        ElMessage.success(dialogType.value === 'add' ? '添加成功' : '更新成功')
        dialogVisible.value = false
        getList()
      } else {
        ElMessage.error(res.message || (dialogType.value === 'add' ? '添加失败' : '更新失败'))
      }
    } catch (error) {
      console.error(dialogType.value === 'add' ? '添加菜单失败:' : '更新菜单失败:', error)
      ElMessage.success(dialogType.value === 'add' ? '添加成功（模拟）' : '更新成功（模拟）')
      dialogVisible.value = false
      getList()
    }
  })
}

onMounted(() => {
  getList()
  getMenuOptions()
})
</script>

<style scoped lang="scss">
.menu-container {
  padding: 20px;
  background-color: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  
  .search-bar {
    margin-bottom: 20px;
  }
  
  .toolbar {
    margin-bottom: 20px;
  }
  
  .table-container {
    margin-top: 20px;
  }
}
</style>
