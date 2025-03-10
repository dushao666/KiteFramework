import request from '../utils/request'
import { ApiResponse } from '../types/api'

export function importExcel(formData: FormData): Promise<ApiResponse<any>> {
  return request({
    url: 'inventoryExcel/excelImport',
    method: 'POST',
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    data: formData
  })
}

export function getImportHistory(params: any): Promise<ApiResponse<any>> {
  return request({
    url: 'inventoryExcel/history',
    method: 'GET',
    params
  })
}

// 查询参数接口
export interface QueryParams {
  pageIndex: number
  pageSize: number
  dimension: string
  year: number
  month: number
  positionKeyword?: string  // 添加可选的岗位关键字参数
}

// 获取SKU维度的明细
export function getSkuDetail(params: QueryParams): Promise<ApiResponse<any>> {
  return request({
    url: 'Inventory/skuDetail',
    method: 'GET',
    params
  })
}

// 获取岗位维度的明细
export function getPositionDetail(params: QueryParams): Promise<ApiResponse<any>> {
  return request({
    url: 'Inventory/positionDetail',
    method: 'GET',
    params
  })
}

// 查询结果接口
export interface QueryResult<T> {
  items: T[]
  total: number
}

// SKU维度数据接口
export interface SkuDimensionData {
  warehouse: string
  sku: string
  packaging: string
  backlogQuantity: number
  inventoryAmount: number
  backlogAmount: number
  backlogRate: number
  month: string
}

// 岗位维度数据接口
export interface PositionDimensionData {
  warehouse: string
  position: string
  backlogQuantity: number
  inventoryAmount: number
  backlogAmount: number
  backlog90Days: number
  backlog180Days: number
  backlog270Days: number
  backlog365Days: number
  backlogRate: number
  month: string
}

// 合并查询方法
export function queryDetail(params: QueryParams): Promise<ApiResponse<any>> {
  const url = params.dimension === 'sku' ? 'Inventory/skuDetail' : 'Inventory/positionDetail'
  return request({
    url,
    method: 'GET',
    params: {
      pageIndex: params.pageIndex,
      pageSize: params.pageSize
    }
  })
}

// 删除指定批次的数据
export function deleteByBatchNo(batchNo: string): Promise<ApiResponse<any>> {
  return request({
    url: `inventoryExcel/batch/${batchNo}`,
    method: 'DELETE'
  })
}
