// 通用API响应类型
export interface ApiResponse<T> {
  result: boolean;
  message: string | null;
  data: T;
  code: number;
  total: number;
  msg: string;
  success: boolean;
}

// 登录响应数据类型
export interface LoginResponseData {
  UserName: string;
  UserId: string;
  BearerToken: string;
  RefreshToken: string;
}

// 文件类型
export interface FileItem {
    Id: string;
    Name: string;
    FileFullName: string;
    Size: string;
    Path: string;
    FileType: string;
    Md5Hash: string;
    FolderId: string;
    UpdateTime?: string;
}

// 文件夹类型
export interface FolderItem {
    Id: string;
    Name: string;
    ParentId: string | null;
    UserId: string;
    Children: FolderItem[];
    Files: FileItem[];
    UpdateTime?: string;
    FileType?: string;
    Path?: string;
    type?: 'folder' | 'file';
    parentFolders?: FolderItem[];  // 存储父文件夹路径
}

// 创建文件夹参数类型
export interface FolderParams {
  id?: string;
  name: string;
  parentId?: string | null;
} 

// 上传文件参数类型
export interface FileUploadParams {
  file: File;
  folderId: string;
  id?: string | 0;
}

// 搜索文件参数类型
export interface FolderSearchParams {
  keyword?: string;
  fileType?: string;
}

export interface ApiErrorResponse {
  code: number
  message: string
  data?: any
}