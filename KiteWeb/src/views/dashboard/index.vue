<template>
  <div class="dashboard-container">
    <el-row :gutter="20">
      <el-col :span="6" v-for="(card, index) in statisticsCards" :key="index">
        <el-card class="statistics-card" :body-style="{ padding: '20px' }">
          <div class="card-content">
            <div class="icon-container" :style="{ backgroundColor: card.color }">
              <i :class="card.icon"></i>
            </div>
            <div class="data-container">
              <div class="data-title">{{ card.title }}</div>
              <div class="data-value">{{ card.value }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" class="chart-row">
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <div class="card-header">
              <span>系统访问量</span>
            </div>
          </template>
          <div class="chart-container" ref="visitChartRef"></div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <div class="card-header">
              <span>数据统计</span>
            </div>
          </template>
          <div class="chart-container" ref="dataChartRef"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" class="table-row">
      <el-col :span="24">
        <el-card class="table-card">
          <template #header>
            <div class="card-header">
              <span>最近操作记录</span>
            </div>
          </template>
          <el-table :data="operationLogs" style="width: 100%" stripe>
            <el-table-column prop="username" label="用户" width="120" />
            <el-table-column prop="operation" label="操作" width="180" />
            <el-table-column prop="ip" label="IP地址" width="140" />
            <el-table-column prop="status" label="状态" width="100">
              <template #default="scope">
                <el-tag :type="scope.row.status === '成功' ? 'success' : 'danger'">
                  {{ scope.row.status }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="time" label="操作时间" width="180" />
            <el-table-column prop="description" label="详细描述" />
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { NAMES } from '../../constants'

// 统计卡片数据
const statisticsCards = ref([
  {
    title: '用户总数',
    value: '1,234',
    icon: 'el-icon-user',
    color: '#409EFF'
  },
  {
    title: '今日访问',
    value: '423',
    icon: 'el-icon-view',
    color: '#67C23A'
  },
  {
    title: '数据总量',
    value: '8,546',
    icon: 'el-icon-document',
    color: '#E6A23C'
  },
  {
    title: '系统消息',
    value: '12',
    icon: 'el-icon-bell',
    color: '#F56C6C'
  }
])

// 操作日志数据
const operationLogs = ref([
  {
    username: 'admin',
    operation: '添加菜单',
    ip: '192.168.1.1',
    status: '成功',
    time: '2025-03-10 15:30:24',
    description: `添加了${NAMES.APP_NAME}系统管理菜单`
  },
  {
    username: 'test',
    operation: '登录系统',
    ip: '192.168.1.2',
    status: '成功',
    time: '2025-03-10 15:20:10',
    description: '用户登录成功'
  },
  {
    username: 'user01',
    operation: '修改用户信息',
    ip: '192.168.1.3',
    status: '成功',
    time: '2025-03-10 15:15:45',
    description: '修改了用户基本信息'
  },
  {
    username: 'user02',
    operation: '删除菜单',
    ip: '192.168.1.4',
    status: '失败',
    time: '2025-03-10 15:10:30',
    description: '尝试删除有子菜单的菜单项'
  },
  {
    username: 'admin',
    operation: '系统配置',
    ip: '192.168.1.1',
    status: '成功',
    time: '2025-03-10 15:05:20',
    description: '修改了系统基本配置'
  }
])

// 图表容器引用
const visitChartRef = ref<HTMLElement | null>(null)
const dataChartRef = ref<HTMLElement | null>(null)

onMounted(() => {
  console.log('仪表盘已加载')
})

onUnmounted(() => {
  console.log('仪表盘已卸载')
})
</script>

<style scoped lang="scss">
.dashboard-container {
  padding: 20px;
  
  .statistics-card {
    margin-bottom: 20px;
    
    .card-content {
      display: flex;
      align-items: center;
      
      .icon-container {
        width: 60px;
        height: 60px;
        border-radius: 50%;
        display: flex;
        justify-content: center;
        align-items: center;
        margin-right: 15px;
        
        i {
          font-size: 24px;
          color: #fff;
        }
      }
      
      .data-container {
        flex: 1;
        
        .data-title {
          font-size: 14px;
          color: #909399;
          margin-bottom: 8px;
        }
        
        .data-value {
          font-size: 24px;
          font-weight: bold;
          color: #303133;
        }
      }
    }
  }
  
  .chart-row {
    margin-bottom: 20px;
    
    .chart-card {
      .card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
      
      .chart-container {
        height: 300px;
      }
    }
  }
  
  .table-row {
    .table-card {
      .card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
    }
  }
}
</style>
