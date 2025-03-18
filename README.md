# Kite Framework

<div align="center">
  <img src="KiteWeb\src\assets\icon\logo.png" alt="Kite Framework Logo" width="200">
  <h3>轻量级全栈开发框架</h3>
</div>

## 📋 目录

- [项目简介](#项目简介)
- [技术栈](#技术栈)
- [项目结构](#项目结构)
- [快速开始](#快速开始)
- [功能特性](#功能特性)
- [开发指南](#开发指南)
- [部署方案](#部署方案)
- [贡献指南](#贡献指南)
- [许可证](#许可证)

## 📖 项目简介

Kite Framework 是一个基于 .NET Core 和 Vue.js 的全栈开发框架，旨在提供一个轻量级、高性能、易于扩展的开发平台。框架采用前后端分离的架构，后端使用 .NET Core 构建 RESTful API，前端使用 Vue.js 和 Element Plus 构建现代化的用户界面。

特点：
- **模块化设计**：易于扩展和维护
- **CQRS 架构**：命令和查询职责分离，提高系统的可扩展性和性能
- **完善的权限控制**：基于角色的访问控制（RBAC）
- **统一的响应格式**：使用 AjaxResponse 统一前后端交互格式
- **前端组件化**：提供丰富的基础组件和业务组件

## 🔧 技术栈

### 后端 (KiteServer)
- **.NET Core**：跨平台的高性能框架
- **Entity Framework Core**：ORM 框架
- **MediatR**：中介者模式实现，用于实现 CQRS
- **Mapster**：对象映射工具
- **Swagger**：API 文档生成工具

### 前端 (KiteWeb)
- **Vue 3**：渐进式 JavaScript 框架
- **TypeScript**：JavaScript 的超集，提供类型检查
- **Element Plus**：基于 Vue 3 的 UI 组件库
- **Vite**：现代化的前端构建工具
- **Axios**：基于 Promise 的 HTTP 客户端
- **Pinia**：Vue 的状态管理库

## 📂 项目结构

### 后端结构 (KiteServer)

```
KiteServer/
├── Api/                   # API 控制器和模型
├── Application/           # 应用服务层，包含命令和查询
├── Domain/                # 领域模型和领域服务
├── DomainShared/          # 领域共享层，定义 DTO 和常量
├── Infrastructure/        # 基础设施层，包含工具类和过滤器
├── Repository/            # 数据访问仓储
└── kiteServer.sln         # 解决方案文件
```

#### 核心目录说明

- **Api**：包含控制器和 API 模型，负责处理 HTTP 请求并返回响应
- **Application**：应用服务层，实现 CQRS 模式，包含命令（Commands）和查询（Queries）
- **Domain**：包含领域模型、聚合根、实体和领域服务
- **Infrastructure**：提供基础设施支持，如过滤器、工具类和中间件

### 前端结构 (KiteWeb)

```
KiteWeb/
├── public/               # 静态资源
├── src/
│   ├── api/              # API 请求封装
│   ├── assets/           # 资源文件（图片、字体等）
│   ├── components/       # 公共组件
│   ├── composables/      # 组合式函数
│   ├── hooks/            # 自定义钩子
│   ├── layout/           # 布局组件
│   ├── router/           # 路由配置
│   ├── store/            # 状态管理
│   ├── styles/           # 全局样式
│   ├── utils/            # 工具函数
│   ├── views/            # 页面视图
│   ├── App.vue           # 根组件
│   └── main.ts           # 入口文件
├── vite.config.ts        # Vite 配置
├── package.json          # 项目依赖
└── tsconfig.json         # TypeScript 配置
```

#### 核心目录说明

- **api**：封装 API 请求，按模块组织
- **components**：公共组件，可复用于多个页面
- **layout**：布局组件，定义页面的整体结构
- **views**：页面组件，按模块组织
- **store**：基于 Pinia 的状态管理

## 🚀 快速开始

### 环境要求

- .NET Core SDK 8.0 或更高版本
- Node.js 20.0 或更高版本
- MySQL 8.0 或更高版本（或其他支持的数据库）

### 后端启动

1. 克隆项目到本地

```bash
git clone https://github.com/dushaoqi666/kiteFramework.git
cd kiteFramework
```

2. 配置数据库连接

修改 `KiteServer/Api/appsettings.json` 中的数据库连接字符串。

3. 运行后端项目

```bash
cd KiteServer
dotnet restore
dotnet run --project Api
```

### 前端启动

1. 安装依赖

```bash
cd KiteWeb
npm install # 或 yarn install 或 pnpm install
```

2. 开发环境运行

```bash
npm run dev # 或 yarn dev 或 pnpm dev
```

3. 生产环境构建

```bash
npm run build # 或 yarn build 或 pnpm build
```

## 🧩 功能特性

### 系统功能
- **用户管理**：用户的增删改查、分配角色、分配岗位、重置密码等
- **角色管理**：角色的增删改查、分配菜单、分配权限等
- **菜单管理**：菜单的增删改查、菜单排序等
- **岗位管理**：岗位的增删改查等
- **权限管理**：基于角色的权限控制
- **日志管理**：操作日志、登录日志等
- 其他模块逐步开发完善中.....

### 开发特性
- **CQRS 模式**：命令和查询责任分离，提高系统可扩展性
- **统一响应格式**：所有 API 返回统一的响应格式
- **全局异常处理**：集中处理系统异常，提供友好的错误提示
- **数据验证**：基于模型的数据验证
- **对象映射**：使用 Mapster 进行对象映射
- **代码生成**：提供代码生成工具，快速创建 CRUD 功能

## 📝 开发指南

### 后端开发

#### 添加新的 API 控制器

1. 在 `KiteServer/Api/Controllers` 中创建新的控制器类
2. 实现 CQRS 模式，创建相应的命令和查询类
3. 在控制器中注入 `IMediator` 和相应的 Queries 接口
4. 为每个操作添加适当的 HTTP 方法和路由

示例：

```csharp
[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMyQueries _myQueries;

    public MyController(IMediator mediator, IMyQueries myQueries)
    {
        _mediator = mediator;
        _myQueries = myQueries;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] MyDto queryDto)
    {
        var result = await _myQueries.GetListAsync(queryDto);
        return new JsonResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddMyCommand command)
    {
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }
}
```

#### 添加新的查询

1. 在 `KiteServer/Application/Queries` 中创建查询接口和实现类
2. 实现查询方法，并返回 `AjaxResponse<T>` 类型的结果

#### 添加新的命令

1. 在 `KiteServer/Application/Commands` 中创建命令类和处理器类
2. 命令类应包含所有必要的属性
3. 处理器类应实现 `IRequestHandler<TCommand, TResponse>` 接口

### 前端开发

#### 添加新的 API 调用

1. 在 `KiteWeb/src/api` 目录下创建或修改相应的 API 模块
2. 使用封装好的 axios 实例发起请求

示例：

```typescript
import request from '@/utils/request';

export function getList(params) {
  return request({
    url: '/api/my/list',
    method: 'get',
    params
  });
}

export function add(data) {
  return request({
    url: '/api/my',
    method: 'post',
    data
  });
}
```

#### 添加新的页面

1. 在 `KiteWeb/src/views` 目录下创建新的页面组件
2. 在 `KiteWeb/src/router` 中添加相应的路由配置

## 🚢 部署方案

### Docker 部署

项目提供了 Docker 支持，可以使用 Docker Compose 一键部署整个应用。

1. 在项目根目录下创建 `docker-compose.yml` 文件：

```yaml
version: '3'
services:
  kite-server:
    build:
      context: ./KiteServer
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    depends_on:
      - db
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=Server=db;Database=kite;User=root;Password=yourpassword;

  kite-web:
    build:
      context: ./KiteWeb
      dockerfile: Dockerfile
    ports:
      - "80:80"
    depends_on:
      - kite-server

  db:
    image: mysql:8.0
    environment:
      - MYSQL_ROOT_PASSWORD=yourpassword
      - MYSQL_DATABASE=kite
    volumes:
      - db-data:/var/lib/mysql

volumes:
  db-data:
```

2. 执行部署命令：

```bash
docker-compose up -d
```

### 传统部署

#### 后端部署

1. 发布后端项目：

```bash
cd KiteServer
dotnet publish -c Release -o publish
```

2. 将发布后的文件部署到 IIS 或其他 Web 服务器。

#### 前端部署

1. 构建前端项目：

```bash
cd KiteWeb
npm run build
```

2. 将 `dist` 目录下的文件部署到 Nginx 或其他 Web 服务器。

Nginx 配置示例：

```nginx
server {
    listen       80;
    server_name  your-domain.com;

    location / {
        root   /path/to/dist;
        index  index.html;
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## 🤝 贡献指南

我们欢迎任何形式的贡献，包括但不限于：

- 提交问题和建议
- 改进文档
- 添加新功能
- 修复 bug

贡献流程：

1. Fork 本仓库
2. 创建你的特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交你的更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建一个 Pull Request

## 📄 许可证

本项目采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

---

<div align="center">
  <p>© 2025 Kite Framework Team. All Rights Reserved.</p>
</div> 