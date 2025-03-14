<template>
    <div class="post-container">
        <div class="search-bar">
            <el-form :inline="true" :model="queryParams" class="demo-form-inline">
                <el-form-item label="岗位编码">
                    <el-input v-model="queryParams.code" placeholder="请输入岗位编码" clearable />
                </el-form-item>
                <el-form-item label="岗位名称">
                    <el-input v-model="queryParams.name" placeholder="请输入岗位名称" clearable />
                </el-form-item>
                <el-form-item label="状态">
                    <el-select v-model="queryParams.status" placeholder="岗位状态" clearable style="width: 120px"
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

            <el-table v-loading="loading" :data="postList" border>
                <el-table-column prop="id" width="50" align="center" label="ID" />
                <el-table-column prop="code" label="岗位编码" align="center" />
                <el-table-column prop="name" label="岗位名称" align="center" />
                <el-table-column prop="sort" label="排序" align="center" width="80" />
                <el-table-column prop="status" label="状态" align="center" width="180">
                    <template #default="scope">
                        <div style="display: flex; align-items: center; justify-content: center; gap: 8px;">
                            <el-switch v-model="scope.row.status" :active-value="0" :inactive-value="1"
                                @change="handleStatusChange(scope.row)" />
                        </div>
                    </template>
                </el-table-column>
                <el-table-column prop="remark" label="备注" align="center" show-overflow-tooltip />
                <el-table-column prop="createTime" label="创建时间" align="center" width="230" />
                <el-table-column label="操作" width="200" align="center">
                    <template #default="scope">
                        <div class="operation-buttons">
                            <el-button type="primary" size="small" @click="handleEdit(scope.row)">
                                <el-icon>
                                    <Edit />
                                </el-icon>
                                <span>编辑</span>
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
                <el-pagination v-model:current-page="queryParams.pageNum" v-model:page-size="queryParams.pageSize"
                    :page-sizes="[10, 20, 50, 100]" layout="total, sizes, prev, pager, next, jumper" :total="total"
                    v-if="total > 0" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
            </div>
        </div>

        <!-- 添加/编辑岗位对话框 -->
        <el-dialog :title="dialogTitle" v-model="dialogVisible" width="500px" append-to-body>
            <el-form ref="postFormRef" :model="postForm" :rules="rules" label-width="100px">
                <el-form-item label="岗位编码" prop="code">
                    <el-input v-model="postForm.code" placeholder="请输入岗位编码" />
                </el-form-item>
                <el-form-item label="岗位名称" prop="name">
                    <el-input v-model="postForm.name" placeholder="请输入岗位名称" />
                </el-form-item>
                <el-form-item label="岗位排序" prop="sort">
                    <el-input-number v-model="postForm.sort" :min="0" :max="999" />
                </el-form-item>
                <el-form-item label="岗位状态" prop="status">
                    <el-radio-group v-model="postForm.status">
                        <el-radio :label="0">正常</el-radio>
                        <el-radio :label="1">停用</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="备注" prop="remark">
                    <el-input v-model="postForm.remark" type="textarea" placeholder="请输入备注" />
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
import { getPostList, addPost, updatePost, deletePost, updatePostStatus, PostItem } from '../../../api/post'
import { NAMES } from '../../../constants'
import { Edit, Delete, Plus, Search, Refresh } from '@element-plus/icons-vue'

// 查询参数
const queryParams = reactive({
    code: '',
    name: '',
    status: 0,
    pageNum: 1,
    pageSize: 10
})

// 岗位表单
interface PostFormData extends PostItem {
    sort: number;
    status: number;
}

const postForm = reactive<PostFormData>({
    id: 0,
    code: '',
    name: '',
    sort: 0,
    status: 0,
    remark: ''
})

// 表单校验规则
const rules = {
    code: [{ required: true, message: '岗位编码不能为空', trigger: 'blur' }],
    name: [{ required: true, message: '岗位名称不能为空', trigger: 'blur' }],
    sort: [{ required: true, message: '排序不能为空', trigger: 'blur' }]
}

const loading = ref(false)
const postList = ref<PostItem[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const dialogType = ref('add')
const postFormRef = ref()

// 对话框标题
const dialogTitle = computed(() => {
    return dialogType.value === 'add' ? '添加岗位' : '编辑岗位'
})

// 查询岗位列表
const getList = async () => {
    loading.value = true
    try {
        const res = await getPostList({
            ...queryParams
        })

        if (res.code === 200) {
            postList.value = res.data
            total.value = res.total || res.data.length // 如果后端返回总数，应该使用后端返回的总数
        } else {
            ElMessage.error(res.message || '获取岗位列表失败')
        }
    } catch (error) {
        console.error('获取岗位列表失败:', error)
        ElMessage.error('获取岗位列表失败')
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
    queryParams.code = ''
    queryParams.name = ''
    queryParams.status = 0
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

// 添加岗位
const handleAdd = () => {
    dialogType.value = 'add'
    resetForm()
    dialogVisible.value = true
}

// 编辑岗位
const handleEdit = (row: PostItem) => {
    dialogType.value = 'edit'
    resetForm()
    Object.assign(postForm, row)
    dialogVisible.value = true
}

// 删除岗位
const handleDelete = (row: PostItem) => {
    ElMessageBox.confirm('确定要删除该岗位吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(async () => {
        try {
            const res = await deletePost(row.id)
            if (res.code === 200) {
                ElMessage.success('删除成功')
                getList()
            } else {
                ElMessage.error(res.message || '删除失败')
            }
        } catch (error) {
            console.error('删除岗位失败:', error)
            ElMessage.error('删除失败')
        }
    }).catch(() => { })
}

// 处理状态变更
const handleStatusChange = async (row: PostItem) => {
    const newStatus = row.status // switch 已经改变了状态值
    const statusText = newStatus === 0 ? '启用' : '停用'
    const originalStatus = newStatus === 0 ? 1 : 0 // 保存原始状态用于恢复

    try {
        const res = await updatePostStatus(row.id, newStatus)
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
        console.error('更新岗位状态失败:', error)
        ElMessage.error(`${statusText}失败`)
        // 恢复原状态
        row.status = originalStatus
    }
}

// 重置表单
const resetForm = () => {
    postForm.id = 0
    postForm.code = ''
    postForm.name = ''
    postForm.sort = 0
    postForm.status = 0
    postForm.remark = ''

    if (postFormRef.value) {
        postFormRef.value.resetFields()
    }
}

// 提交表单
const submitForm = async () => {
    if (!postFormRef.value) return

    await postFormRef.value.validate(async (valid: boolean) => {
        if (!valid) return

        try {
            const api = dialogType.value === 'add' ? addPost : updatePost
            const res = await api(postForm)

            if (res.code === 200) {
                ElMessage.success(dialogType.value === 'add' ? '添加成功' : '更新成功')
                dialogVisible.value = false
                getList()
            } else {
                ElMessage.error(res.message || (dialogType.value === 'add' ? '添加失败' : '更新失败'))
            }
        } catch (error) {
            console.error(dialogType.value === 'add' ? '添加岗位失败:' : '更新岗位失败:', error)
            ElMessage.error(dialogType.value === 'add' ? '添加失败' : '更新失败')
        }
    })
}

onMounted(() => {
    getList()
})
</script>

<style scoped lang="scss">
.post-container {
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
    flex-wrap: wrap;

    .el-button {
        padding: 4px 8px;
        margin: 2px 0;

        .el-icon {
            margin-right: 4px;
        }
    }
}
</style>