<template>
    <main-layout>
        <div class="import-detail">
            <el-card class="detail-card">
                <template #header>
                    <div class="header">
                        <div class="left">
                            <span class="title">导入明细查询</span>
                            <el-divider direction="vertical" />
                            <div class="select-group">
                                <span class="select-label">查看维度：</span>
                                <el-select v-model="dimension" placeholder="请选择维度" class="dimension-select"
                                    @change="handleDimensionChange">
                                    <el-option label="SKU维度" value="sku">
                                        <template #default>
                                            <div class="option-content">
                                                <el-icon class="option-icon">
                                                    <Goods />
                                                </el-icon>
                                                <div class="option-text">
                                                    <div class="label">SKU维度</div>
                                                    <div class="desc">查看各SKU的库存和积压天数</div>
                                                </div>
                                            </div>
                                        </template>
                                    </el-option>
                                    <el-option label="岗位维度" value="position">
                                        <template #default>
                                            <div class="option-content">
                                                <el-icon class="option-icon">
                                                    <User />
                                                </el-icon>
                                                <div class="option-text">
                                                    <div class="label">岗位维度</div>
                                                    <div class="desc">查看各岗位的积压金额</div>
                                                </div>
                                            </div>
                                        </template>
                                    </el-option>
                                </el-select>

                                <!-- 岗位关键字搜索框 -->
                                <template v-if="dimension === 'position'">
                                    <el-divider direction="vertical" />
                                    <span class="select-label">岗位关键字：</span>
                                    <el-input v-model="queryParams.positionKeyword" placeholder="请输入岗位关键字" clearable
                                        class="search-input" style="width: 200px" @clear="handlePositionSearch"
                                        @keyup.enter="handlePositionSearch">
                                        <template #append>
                                            <el-button type="primary" :icon="Search" @click="handlePositionSearch"
                                                size="small">

                                            </el-button>
                                        </template>
                                    </el-input>
                                </template>

                                <el-divider direction="vertical" />

                                <!-- 添加年月选择 -->
                                <span class="select-label">统计年份：</span>
                                <el-select v-model="queryParams.year" placeholder="选择年份" style="width: 120px"
                                    @change="handleYearChange">
                                    <el-option v-for="year in yearOptions" :key="year" :label="`${year}年`"
                                        :value="year" />
                                </el-select>

                                <span class="select-label" style="margin-left: 16px">统计月份：</span>
                                <el-select v-model="queryParams.month" placeholder="选择月份" style="width: 120px"
                                    @change="handleMonthChange">
                                    <el-option v-for="month in 12" :key="month" :label="`${month}月`" :value="month" />
                                </el-select>
                            </div>
                        </div>
                        <div class="right">
                            <el-button type="primary" :icon="Refresh" @click="refreshData">刷新</el-button>
                        </div>
                    </div>
                </template>

                <el-table v-loading="loading" :data="tableData" stripe style="width: 100%" border>
                    <!-- SKU维度的列 -->
                    <template v-if="dimension === 'sku'">
                        <el-table-column prop="warehouse" label="仓库" align="center" />
                        <el-table-column prop="sku" label="SKU" min-width="120" align="center" />
                        <el-table-column prop="packaging" label="包装" width="100" align="center" />
                        <el-table-column prop="backlogQuantity" label="积压数量" align="center" width="100">
                            <template #default="{ row }">
                                <span class="number-cell">{{ row.backlogQuantity }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="inventoryAmount" label="库存金额" align="center" width="120">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatMoney(row.inventoryAmount) }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="backlogAmount" label="积压金额" align="center" width="120">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatMoney(row.backlogAmount) }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="backlogRate" label="积压率" align="center" width="100">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatPercent(row.backlogRate) }}</span>
                            </template>
                        </el-table-column>
                    </template>

                    <!-- 岗位维度的列 -->
                    <template v-else>
                        <el-table-column prop="warehouse" label="仓库" width="120" align="center" />
                        <el-table-column prop="position" label="岗位" min-width="120" align="center" />
                        <el-table-column prop="backlogQuantity" label="积压数量" width="100" align="center">
                            <template #default="{ row }">
                                <span class="number-cell">{{ row.backlogQuantity }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="inventoryAmount" label="库存金额" width="120" align="center">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatMoney(row.inventoryAmount) }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="backlogAmount" label="积压金额" width="120" align="center">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatMoney(row.backlogAmount) }}</span>
                            </template>
                        </el-table-column>

                        <!-- 积压天数分布 -->
                        <el-table-column label="积压天数分布" align="center">
                            <el-table-column label="90天" align="center">
                                <el-table-column prop="backlog90Days" label="金额" width="100" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatMoney(row.backlog90Days) }}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="backlogRate90Days" label="占比" width="80" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatPercent(row.backlogRate90Days) }}</span>
                                    </template>
                                </el-table-column>
                            </el-table-column>

                            <el-table-column label="180天" align="center">
                                <el-table-column prop="backlog180Days" label="金额" width="100" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatMoney(row.backlog180Days) }}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="backlogRate180Days" label="占比" width="80" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatPercent(row.backlogRate180Days) }}</span>
                                    </template>
                                </el-table-column>
                            </el-table-column>

                            <el-table-column label="270天" align="center">
                                <el-table-column prop="backlog270Days" label="金额" width="100" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatMoney(row.backlog270Days) }}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="backlogRate270Days" label="占比" width="80" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatPercent(row.backlogRate270Days) }}</span>
                                    </template>
                                </el-table-column>
                            </el-table-column>

                            <el-table-column label="365天" align="center">
                                <el-table-column prop="backlog365Days" label="金额" width="100" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatMoney(row.backlog365Days) }}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="backlogRate365Days" label="占比" width="80" align="center">
                                    <template #default="{ row }">
                                        <span class="number-cell">{{ formatPercent(row.backlogRate365Days) }}</span>
                                    </template>
                                </el-table-column>
                            </el-table-column>
                        </el-table-column>

                        <el-table-column prop="backlogRate" label="总积压率" align="center" width="100">
                            <template #default="{ row }">
                                <span class="number-cell">{{ formatPercent(row.backlogRate) }}</span>
                            </template>
                        </el-table-column>
                    </template>

                    <!-- 共同的列 -->
                    <el-table-column prop="month" label="统计月份" align="center" width="100" />
                </el-table>

                <div class="pagination">
                    <el-pagination v-model:current-page="queryParams.pageIndex" v-model:page-size="queryParams.pageSize"
                        :page-sizes="[10, 20, 50, 100]" :total="total" layout="total, sizes, prev, pager, next"
                        @size-change="handleSizeChange" @current-change="handleCurrentChange" v-if="total > 0" />
                </div>
            </el-card>
        </div>
    </main-layout>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Refresh, Goods, User, Search } from '@element-plus/icons-vue'
import {
    getSkuDetail,
    getPositionDetail,
    type QueryParams,
    type SkuDimensionData,
    type PositionDimensionData
} from '../../api/excel'
import MainLayout from '../../components/layout/MainLayout.vue'

// 查询参数
const queryParams = ref<QueryParams>({
    pageIndex: 1,
    pageSize: 10,
    dimension: 'sku',
    year: new Date().getFullYear(),  // 默认当前年
    month: new Date().getMonth() + 1,  // 默认当前月
    positionKeyword: ''  // 添加岗位关键字参数
})

// 页面状态
const dimension = ref('sku')
const loading = ref(false)
const tableData = ref<(SkuDimensionData | PositionDimensionData)[]>([])
const total = ref(0)

// 生成年份选项（最近5年）
const yearOptions = computed(() => {
    const currentYear = new Date().getFullYear()
    const years = []
    for (let i = 0; i < 5; i++) {
        years.push(currentYear - i)
    }
    return years
})

// 查询数据
async function fetchData() {
    loading.value = true
    try {
        let res
        if (dimension.value === 'sku') {
            res = await getSkuDetail(queryParams.value)
        } else {
            res = await getPositionDetail(queryParams.value)
        }

        if (res.code === 200) {
            tableData.value = res.data
            total.value = res.total
            ElMessage.success(res.message)
        } else {
            ElMessage.error(res.message || '获取数据失败')
        }
    } catch (error) {
        console.error('获取数据失败:', error)
        ElMessage.error('获取数据失败')
    } finally {
        loading.value = false
    }
}

// 添加岗位搜索处理方法
function handlePositionSearch() {
    queryParams.value.pageIndex = 1  // 重置页码
    fetchData()
}

// 维度变化处理
function handleDimensionChange() {
    queryParams.value.pageIndex = 1
    queryParams.value.positionKeyword = ''  // 切换维度时清空关键字
    fetchData()
}

// 页码变化
function handleCurrentChange(val: number) {
    queryParams.value.pageIndex = val
    fetchData()
}

// 每页条数变化
function handleSizeChange(val: number) {
    queryParams.value.pageSize = val
    queryParams.value.pageIndex = 1
    fetchData()
}

// 刷新数据
function refreshData() {
    fetchData()
    ElMessage.success('数据已刷新')
}

// 格式化方法
function formatMoney(value: number) {
    return new Intl.NumberFormat('zh-CN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(value)
}

function formatPercent(value: number) {
    return `${(value * 1.0).toFixed(2)}%`
}

// 年份变化处理
function handleYearChange() {
    fetchData()
}

// 月份变化处理
function handleMonthChange() {
    fetchData()
}

// 初始化加载
onMounted(() => {
    fetchData()
})
</script>

<style scoped lang="scss">
.import-detail {
    padding: 12px;

    .detail-card {
        .header {
            padding: 8px 12px;
            display: flex;
            justify-content: space-between;
            align-items: center;

            .left {
                display: flex;
                align-items: center;
                gap: 16px;

                .title {
                    font-size: 16px;
                    font-weight: 500;
                    color: #303133;
                }

                .select-group {
                    display: flex;
                    align-items: center;
                    gap: 8px;

                    .select-label {
                        font-size: 14px;
                        color: #606266;
                        white-space: nowrap;
                    }

                    .el-divider--vertical {
                        margin: 0 16px;
                    }
                }
            }
        }

        :deep(.el-card__body) {
            padding: 12px;
        }

        :deep(.el-table) {
            margin: 12px 0;

            .el-table__header th {
                background-color: #f5f7fa;
                color: #606266;
                font-weight: 600;
            }

            .number-cell {
                font-family: Monaco, Menlo, Consolas, "Courier New", monospace;
                color: #606266;
            }
        }

        .pagination {
            margin-top: 12px;
            display: flex;
            justify-content: flex-end;
        }
    }
}

// 选择框样式
.dimension-select {
    width: 200px;

    :deep(.el-select-dropdown__item) {
        padding: 8px 12px;
        height: auto;
        line-height: 1.4;
    }
}

// 选项内容样式
:deep(.option-content) {
    display: flex;
    align-items: flex-start;
    gap: 8px;
    white-space: normal;

    .option-icon {
        margin-top: 3px;
        font-size: 16px;
        color: #909399;
        flex-shrink: 0;
    }

    .option-text {
        flex: 1;
        min-width: 0;

        .label {
            font-size: 14px;
            color: #303133;
            margin-bottom: 2px;
        }

        .desc {
            font-size: 12px;
            color: #909399;
        }
    }
}

:deep(.el-select-dropdown__item.selected) {
    .option-icon {
        color: var(--el-color-primary);
    }

    .option-text .label {
        color: var(--el-color-primary);
    }
}

:deep(.el-select-dropdown__item:hover) {
    .option-icon {
        color: var(--el-color-primary);
    }
}

// 修改搜索框样式
.search-input {
    :deep(.el-input-group__append) {
        padding: 0;
        background-color: var(--el-color-primary);

        .el-button {
            margin: 0;
            border: none;
            height: 32px;
            padding: 8px 16px;
            color: white;

            .el-icon {
                margin-right: 4px;
            }

            &:hover {
                background-color: var(--el-color-primary-dark-2);
                color: white;
            }
        }
    }

    :deep(.el-input__wrapper) {
        &.is-focus {
            box-shadow: 0 0 0 1px var(--el-color-primary) inset;
        }
    }
}
</style>