<template>
  <div class="menu-container">
    <div class="search-bar">
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="关键字">
          <el-input v-model="queryParams.keyword" placeholder="请输入菜单名称或路径" clearable />
        </el-form-item>
        <el-form-item label="包含隐藏">
          <el-switch v-model="queryParams.includeHidden" @change="handleQuery" />
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
        <el-button type="primary" @click="() => handleAdd()">
          <el-icon>
            <Plus />
          </el-icon>
          <span>{{ NAMES.BUTTONS.ADD }}</span>
        </el-button>
        <el-button @click="toggleExpand" type="warning">
          <el-icon>
            <component :is="isExpanded ? FolderOpened : Folder" />
          </el-icon>
          <span>{{ isExpanded ? '折叠所有' : '展开所有' }}</span>
        </el-button>
      </div>

      <el-table ref="tableRef" v-loading="loading" :data="menuList" row-key="id" border :default-expand-all="isExpanded"
        :tree-props="{ children: 'children' }">
        <el-table-column prop="name" label="菜单名称" align="left" />
        <el-table-column prop="path" label="路径" align="center" />
        <el-table-column prop="icon" label="图标" align="center">
          <template #default="scope">
            <i :class="scope.row.icon"></i>
          </template>
        </el-table-column>
        <el-table-column prop="sort" label="排序" align="center" />
        <el-table-column prop="isHidden" label="是否隐藏" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.isHidden ? 'danger' : 'success'">
              {{ scope.row.isHidden ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" align="center">
          <template #default="scope">
            <div class="operation-buttons">
              <el-button type="primary" size="small" @click="handleEdit(scope.row)">
                <el-icon>
                  <Edit />
                </el-icon>
                <span>编辑</span>
              </el-button>
              <el-button type="success" size="small" @click="handleAdd(scope.row)">
                <el-icon>
                  <Plus />
                </el-icon>
                <span>添加</span>
              </el-button>
              <el-button type="danger" size="small" @click="handleDelete(scope.row)"
                :disabled="scope.row.children?.length > 0 || scope.row.path === '/home'">
                <el-icon>
                  <Delete />
                </el-icon>
                <span>删除</span>
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 添加/编辑菜单对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="600px" append-to-body>
      <el-form ref="menuFormRef" :model="menuForm" :rules="rules" label-width="120px">
        <el-form-item label="上级菜单">
          <el-tree-select v-model="menuForm.parentId" :data="menuOptions" check-strictly default-expand-all
            node-key="id" :props="{ label: 'name', value: 'id' }" placeholder="请选择上级菜单" clearable />
        </el-form-item>
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="菜单路径" prop="path">
          <el-input v-model="menuForm.path" placeholder="请输入菜单路径，如: /system/menu" />
          <div class="form-tip">菜单路径必须以 / 开头，如: /system/menu</div>
        </el-form-item>
        <el-form-item label="组件路径" prop="component">
          <el-input v-model="menuForm.component" placeholder="请输入组件路径，如: system/menu/index" />
          <div class="form-tip">
            <p>1. 顶级菜单可填写 Layout 或留空</p>
            <p>2. 组件路径相对于 views 目录，无需添加开头的 /</p>
            <p>3. 无需添加 .vue 后缀</p>
            <p>4. 示例: system/menu/index</p>
          </div>
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
        
        <el-divider content-position="center">路由元数据</el-divider>
        
        <el-form-item label="页面标题">
          <el-input v-model="menuForm.meta.title" placeholder="请输入页面标题" />
        </el-form-item>
        <el-form-item label="是否需要认证">
          <el-switch v-model="menuForm.meta.requiresAuth" />
        </el-form-item>
        <el-form-item label="是否缓存组件">
          <el-switch v-model="menuForm.meta.keepAlive" />
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
import { getMenuTree, addMenu, updateMenu, deleteMenu, MenuItem, getMenuList } from '../../../api/menu'
import { NAMES } from '../../../constants'
import { Folder, FolderOpened, Edit, Plus, Delete, Search, Refresh } from '@element-plus/icons-vue'

// 查询参数
const queryParams = reactive({
  keyword: '',
  includeHidden: false
})

// 菜单表单
interface MenuFormData extends MenuItem {
  sort: number;
  isHidden: boolean;
  meta: {
    title: string;
    requiresAuth: boolean;
    keepAlive: boolean;
  };
}

const menuForm = reactive<MenuFormData>({
  id: undefined as unknown as number,
  name: '',
  path: '',
  icon: '',
  component: '',
  parentId: undefined as unknown as number,
  sort: 0,
  isHidden: false,
  meta: {
    title: '',
    requiresAuth: true,
    keepAlive: false
  }
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
const tableRef = ref()
const isExpanded = ref(true)

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加菜单' : '编辑菜单'
})

// 切换展开/折叠状态
const toggleExpand = () => {
  isExpanded.value = !isExpanded.value
  
  // 手动设置表格的展开状态
  if (tableRef.value) {
    const rows = tableRef.value.store.states.data.value
    rows.forEach((row: any) => {
      tableRef.value.toggleRowExpansion(row, isExpanded.value)
    })
  }
}

// 获取菜单选项（用于上级菜单选择）
const getMenuOptions = async () => {
  try {
    const res = await getMenuTree()
    if (res.code === 200) {
      // 添加顶级菜单选项
      const topMenu = { id: 0, name: '顶级菜单', path: '' }
      menuOptions.value = [topMenu, ...res.data]
    } else {
      ElMessage.error(res.message || '获取菜单选项失败')
    }
  } catch (error) {
    // 获取菜单选项失败
    ElMessage.error('获取菜单选项失败')
  }
}

// 查询菜单列表
const getList = async () => {
  loading.value = true
  try {
    // 如果有搜索条件，使用 getMenuList
    if (queryParams.keyword || queryParams.includeHidden) {
      const res = await getMenuList({
        keyword: queryParams.keyword,
        includeHidden: queryParams.includeHidden
      })

      if (res.code === 200) {
        menuList.value = res.data
      } else {
        ElMessage.error(res.message || '获取菜单列表失败')
      }
    } else {
      // 没有搜索条件，使用 getMenuTree
      const res = await getMenuTree()

      if (res.code === 200) {
        menuList.value = res.data
      } else {
        ElMessage.error(res.message || '获取菜单列表失败')
      }
    }
  } catch (error) {
    // 获取菜单列表失败
    ElMessage.error('获取菜单列表失败')
  } finally {
    loading.value = false
  }
}

// 查询按钮点击事件
const handleQuery = () => {
  getList()
}

// 重置查询条件
const resetQuery = () => {
  queryParams.keyword = ''
  queryParams.includeHidden = false
  getList()
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
  // 检查是否是首页菜单
  if (row.path === '/home') {
    ElMessage.warning('首页菜单不能删除')
    return
  }

  // 检查是否有子菜单
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
      // 删除菜单失败
      ElMessage.error('删除失败')
    }
  }).catch(() => { })
}

// 重置表单
const resetForm = () => {
  menuForm.id = undefined as unknown as number
  menuForm.name = ''
  menuForm.path = ''
  menuForm.component = ''
  menuForm.parentId = undefined as unknown as number
  menuForm.icon = ''
  menuForm.sort = 0
  menuForm.isHidden = false
  menuForm.meta = {
    title: '',
    requiresAuth: true,
    keepAlive: false
  }

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
      // 处理组件路径格式
      if (menuForm.component) {
        // 移除开头的 / 和结尾的 .vue
        menuForm.component = menuForm.component.replace(/^\//, '').replace(/\.vue$/, '')
      }
      
      // 确保路径以 / 开头
      if (menuForm.path && !menuForm.path.startsWith('/')) {
        menuForm.path = `/${menuForm.path}`
      }
      
      // 如果没有设置标题，使用菜单名称
      if (!menuForm.meta.title) {
        menuForm.meta.title = menuForm.name
      }

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
      // 提交表单失败
      ElMessage.error(dialogType.value === 'add' ? '添加失败' : '更新失败')
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
}

/* 操作按钮样式 */
.operation-buttons {
  display: flex;
  justify-content: center;
  gap: 8px;
  flex-wrap: wrap;

  .el-button {
    padding: 4px 8px;
    margin: 2px 0;

    .el-icon {
      margin-right: 4px;
    }
  }
}

.form-tip {
  font-size: 12px;
  color: #909399;
  line-height: 1.2;
  padding-top: 4px;
}
</style>
