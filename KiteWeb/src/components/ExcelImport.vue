<template>
    <div class="import-container">
        <el-card class="upload-card">
            <template #header>
                <div class="card-header">
                    <div class="left">
                        <span>库存积压数据导入</span>
                        <el-divider direction="vertical" />
                        <div class="year-select-group">
                            <span class="label">年份：</span>
                            <el-select v-model="selectedYear" placeholder="选择年份" style="width: 160px">
                                <el-option v-for="year in yearOptions" :key="year" :label="`${year}年度`" :value="year" />
                            </el-select>
                        </div>
                    </div>
                    <el-button type="primary" link @click="downloadTemplate">
                        <el-icon>
                            <Download />
                        </el-icon>下载模板
                    </el-button>
                </div>
            </template>

            <el-upload class="upload-area" drag :auto-upload="false" accept=".xlsx,.xls" :on-change="handleFileChange"
                :show-file-list="false">
                <el-icon class="upload-icon">
                    <Upload />
                </el-icon>
                <div class="upload-text">
                    <span>将文件拖到此处，或</span>
                    <em>点击上传</em>
                </div>
                <template #tip>
                    <div class="upload-tip">支持 .xlsx、.xls 格式的Excel文件</div>
                </template>
            </el-upload>

            <div v-if="selectedFile" class="file-info">
                <el-icon>
                    <Document />
                </el-icon>
                <span class="file-name">{{ selectedFile.name }}</span>
                <el-button type="primary" @click="handleImport" :loading="importing">
                    开始导入
                </el-button>
            </div>
        </el-card>

        <!-- 导入历史记录 -->
        <el-card class="history-card">
            <template #header>
                <div class="card-header">
                    <span>导入历史</span>
                    <el-button type="primary" link @click="refreshHistory">
                        <el-icon>
                            <Refresh />
                        </el-icon>刷新
                    </el-button>
                </div>
            </template>

            <el-table :data="importHistory" stripe style="width: 100%" border>
                <el-table-column prop="fileName" label="文件名" align="center" />
                <el-table-column prop="recordCount" label="导入记录" align="center" />
                <el-table-column prop="importTime" label="导入时间" align="center" />
                <el-table-column prop="status" label="状态" align="center">
                    <template #default="scope">
                        <el-tag :type="scope.row.status === '成功' ? 'success' : 'danger'">
                            {{ scope.row.status }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center" width="200">
                    <template #default="scope">
                        <el-button type="primary" link @click="downloadResult(scope.row)">
                            下载结果
                        </el-button>
                        <el-divider direction="vertical" />
                        <el-popconfirm title="确定要删除导入历史及导入明细吗？" confirm-button-text="确定" cancel-button-text="取消"
                            @confirm="handleDelete(scope.row)">
                            <template #reference>
                                <el-button type="danger" link>
                                    删除
                                </el-button>
                            </template>
                        </el-popconfirm>
                    </template>
                </el-table-column>
            </el-table>
            <div class="pagination">
                <el-pagination v-model:current-page="queryParams.pageIndex" v-model:page-size="queryParams.pageSize"
                    :page-sizes="[10, 20, 50, 100]" :total="total" layout="total, sizes, prev, pager, next"
                    @size-change="handleSizeChange" @current-change="handleCurrentChange" v-if="total > 0" />
            </div>
        </el-card>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Document, Download, Upload, Refresh } from '@element-plus/icons-vue'
import { importExcel, getImportHistory, deleteByBatchNo } from '../api/excel'

const selectedFile = ref<File | null>(null)
const importing = ref(false)
const importHistory = ref([])
// 查询参数
const queryParams = ref<any>({
    pageIndex: 1,
    pageSize: 10
})
const total = ref(0)
// 页码变化
function handleCurrentChange(val: number) {
    queryParams.value.pageIndex = val
    refreshHistory()
}

// 每页条数变化
function handleSizeChange(val: number) {
    queryParams.value.pageSize = val
    queryParams.value.pageIndex = 1
    refreshHistory()
}
// 年份选择
const currentYear = new Date().getFullYear()
const selectedYear = ref(currentYear)

// 生成年份选项（从当前年份开始，往前推5年）
const yearOptions = computed(() => {
    const years = []
    for (let i = 0; i < 5; i++) {
        years.push(currentYear - i)
    }
    return years
})

const handleFileChange = (file: any) => {
    selectedFile.value = file.raw
}

const handleImport = async () => {
    if (!selectedFile.value) {
        ElMessage.warning('请先选择要导入的文件')
        return
    }

    importing.value = true
    try {
        const formData = new FormData()
        formData.append('file', selectedFile.value)
        formData.append('year', selectedYear.value.toString())  // 添加年份参数

        const res = await importExcel(formData)
        if (res.code === 200) {
            if (res.total === 0) {
                ElMessage.warning('请勿重复导入')
            } else {
                ElMessage.success(res.message)
            }
            selectedFile.value = null
        } else {
            ElMessage.error(res.message || '导入失败')
        }
    } catch (error: any) {
        console.error('导入错误:', error)
        ElMessage.error(error.message || '导入失败')

    } finally {
        importing.value = false
        await refreshHistory()
    }
}

const downloadTemplate = () => {
    const baseUrl = import.meta.env.VITE_APP_BASE_URL
    const templatePath = import.meta.env.VITE_APP_TEMPLATE_PATH
    const fullUrl = `${baseUrl}${templatePath}`

    // 创建一个隐藏的 a 标签来下载文件
    const link = document.createElement('a')
    link.href = fullUrl
    link.download = '库存积压维度模版.xlsx' // 设置下载文件名

    try {
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        ElMessage.success('模板下载开始')
    } catch (error) {
        console.error('下载模板失败:', error)
        ElMessage.error('模板下载失败')
    }
}

const refreshHistory = async () => {
    try {
        const res = await getImportHistory(queryParams.value)
        if (res.code === 200) {
            total.value = res.total
            importHistory.value = res.data.map((item: any) => ({
                fileName: item.fileName,
                importTime: item.importTime,
                batchNo: item.batchNo,
                recordCount: item.recordCount,
                filePath: item.filePath,
                status: item.status === 1 ? '成功' : '失败'
            }))
            ElMessage.success('刷新成功')
        }
    } catch (error) {
        console.error('获取历史记录失败:', error)
        ElMessage.error('获取历史记录失败')
    }
}

// 组件加载时初始化
onMounted(() => {
    selectedYear.value = currentYear  // 确保设置为当前年份
    refreshHistory()
})

const downloadResult = (row: any) => {
    // TODO: 实现结果下载逻辑
    ElMessage.success(`正在下载 ${row.fileName} 的导入结果`)
    const baseUrl = import.meta.env.VITE_APP_BASE_URL
    const fileBasePath = import.meta.env.VITE_APP_FILE_PATH
    const fullUrl = `${baseUrl}${fileBasePath}${row.filePath}`

    // 创建一个隐藏的 a 标签来下载文件
    const link = document.createElement('a')
    link.href = fullUrl
    link.download = row.fileName // 设置下载文件名

    try {
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        ElMessage.success('下载成功')
    } catch (error) {
        console.error('下载失败:', error)
        ElMessage.error('下载失败')
    }
}

// 添加删除方法
const handleDelete = async (row: any) => {
    try {
        const res = await deleteByBatchNo(row.batchNo)
        if (res.code === 200) {
            ElMessage.success('删除成功')
            await refreshHistory()
        } else {
            ElMessage.error(res.message || '删除失败')
        }
    } catch (error) {
        console.error('删除失败:', error)
        ElMessage.error('删除失败')
    }
}
</script>

<style scoped lang="scss">
.import-container {
    padding: 12px;

    .upload-card,
    .history-card {
        :deep(.el-card__header) {
            padding: 8px 12px;
        }

        :deep(.el-card__body) {
            padding: 12px;
        }

        &+.history-card {
            margin-top: 12px;
        }
    }

    .upload-card {
        margin-bottom: 20px;

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;

            .left {
                display: flex;
                align-items: center;
                gap: 16px;

                .year-select-group {
                    display: flex;
                    align-items: center;
                    gap: 8px;

                    .label {
                        font-size: 14px;
                        color: #606266;
                    }
                }
            }
        }
    }

    .upload-icon {
        font-size: 48px;
        color: #409eff;
        margin-bottom: 10px;
    }

    .upload-text {
        color: #606266;
        font-size: 14px;
        text-align: center;

        em {
            color: #409eff;
            font-style: normal;
            margin-left: 4px;
        }
    }

    .upload-tip {
        font-size: 12px;
        color: #909399;
        margin-top: 10px;
        text-align: center;
    }

    .file-info {
        margin-top: 20px;
        padding: 10px;
        background-color: #f5f7fa;
        border-radius: 4px;
        display: flex;
        align-items: center;
        gap: 10px;

        .file-name {
            flex: 1;
            color: #606266;
        }
    }

    .history-card {
        .el-table {
            margin-top: 10px;
        }
    }

    .pagination {
        margin-top: 12px;
        display: flex;
        justify-content: flex-end;
    }
}
</style>