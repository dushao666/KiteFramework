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
          <el-button type="primary" @click="handleQuery">{{ NAMES.BUTTONS.SEARCH }}</el-button>
          <el-button @click="resetQuery">{{ NAMES.BUTTONS.RESET }}</el-button>
        </el-form-item>
      </el-form>
    </div>

    <div class="table-container">
      <div class="toolbar">
        <el-button type="primary" @click="() => handleAdd()">{{ NAMES.BUTTONS.ADD }}</el-button>
        <el-button @click="toggleExpand" type="warning">
          {{ isExpanded ? '折叠所有' : '展开所有' }}
          <el-icon class="el-icon--right">
            <component :is="isExpanded ? FolderOpened : Folder" />
          </el-icon>
        </el-button>
      </div>

      <el-table ref="tableRef" v-loading="loading" :data="menuList" row-key="id" border :default-expand-all="isExpanded"
        :tree-props="{ children: 'children' }">
        <el-table-column prop="name" label="菜单名称"  align="left" />
        <el-table-column prop="path" label="路径"  align="center" />
        <el-table-column prop="icon" label="图标"  align="center">
          <template #default="scope">
            <i :class="scope.row.icon"></i>
          </template>
        </el-table-column>
        <el-table-column prop="sort" label="排序"  align="center" />
        <el-table-column prop="isHidden" label="是否隐藏"  align="center">
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
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="500px" append-to-body>
      <el-form ref="menuFormRef" :model="menuForm" :rules="rules" label-width="100px">
        <el-form-item label="上级菜单">
          <el-tree-select v-model="menuForm.parentId" :data="menuOptions" check-strictly default-expand-all
            node-key="id" :props="{ label: 'name', value: 'id' }" placeholder="请选择上级菜单" clearable />
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
import { getMenuTree, addMenu, updateMenu, deleteMenu, MenuItem, getMenuList } from '../../../api/menu'
import { NAMES } from '../../../constants'
import { Folder, FolderOpened, Edit, Plus, Delete } from '@element-plus/icons-vue'

// 查询参数
const queryParams = reactive({
  keyword: '',
  includeHidden: false
})

// 菜单表单
interface MenuFormData extends MenuItem {
  sort: number;
  isHidden: boolean;
}

const menuForm = reactive<MenuFormData>({
  id: undefined as unknown as number,
  name: '',
  path: '',
  icon: '',
  parentId: undefined as unknown as number,
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
const tableRef = ref()
const isExpanded = ref(true)

// 对话框标题
const dialogTitle = computed(() => {
  return dialogType.value === 'add' ? '添加菜单' : '编辑菜单'
})

// 切换展开/折叠状态
const toggleExpand = () => {
  isExpanded.value = !isExpanded.value

  // 强制表格重新渲染
  if (tableRef.value) {
    // 获取表格的所有行数据
    const rows = tableRef.value.data || []

    // 递归处理所有行及其子行
    const processRows = (items: MenuItem[]) => {
      items.forEach(row => {
        // 根据当前状态展开或折叠行
        if (isExpanded.value) {
          // 展开行
          tableRef.value.toggleRowExpansion(row, true)
        } else {
          // 折叠行
          tableRef.value.toggleRowExpansion(row, false)
        }

        // 处理子行
        if (row.children && row.children.length > 0) {
          processRows(row.children)
        }
      })
    }

    // 处理所有行
    processRows(rows)
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
    console.error('获取菜单列表失败:', error)
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
      ElMessage.error('删除失败')
    }
  }).catch(() => { })
}

// 重置表单
const resetForm = () => {
  menuForm.id = undefined as unknown as number
  menuForm.name = ''
  menuForm.path = ''
  menuForm.parentId = undefined as unknown as number
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

    :deep(.el-scrollbar) {
      height: 100% !important;
      flex: 1;
    }

    :deep(.el-scrollbar__wrap) {
      height: 100% !important;
    }

    :deep(.el-table) {
      height: 100%;
      display: flex;
      flex-direction: column;

      .el-table__body-wrapper {
        height: 100% !important;
      }

      .el-table__header,
      .el-table__body {
        width: 100% !important;
      }

      /* 让操作列自适应 */
      .el-table__row .cell {
        white-space: nowrap;
      }

      /* 移除表格底部可能的间隙 */
      .el-table__footer-wrapper,
      .el-table__append-wrapper {
        display: none;
      }

      /* 确保表格内容垂直居中 */
      .cell {
        display: flex;
        align-items: center;
        justify-content: center;
      }

      /* 确保表头居中 */
      th .cell {
        justify-content: center;
      }

      /* 修改菜单名称列为左对齐 - 使用更精确的选择器 */
      th:first-child .cell,
      td:first-child .cell {
        justify-content: flex-start;
      }

      /* 确保操作列有足够宽度 */
      .el-table-column--operation .cell {
        min-width: 240px;
      }
    }
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
