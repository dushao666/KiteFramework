import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'
import { viteBuildInfo } from './build/info'
import { configCompressPlugin } from './build/compress'
import { getPluginsList } from './build/plugins'
import { silenceSassWarnings } from './build/silence-sass-warnings'
import fs from 'fs'

// 注意: 如需完整功能，请安装以下依赖:
// npm install --save-dev dayjs

// 拦截控制台警告，在构建过程中不输出特定警告
const originalConsoleWarn = console.warn;
console.warn = function(msg, ...args) {
  // 过滤掉 Sass 警告
  if (typeof msg === 'string' && (
      msg.includes('Deprecation Warning') || 
      msg.includes('sass-lang.com') ||
      msg.includes('legacy-js-api'))) {
    return;
  }
  originalConsoleWarn.call(console, msg, ...args);
};

export default defineConfig(({ mode, command }) => {
  const env = loadEnv(mode, process.cwd())
  
  // 构建相关的环境变量
  const { VITE_CDN = false, VITE_COMPRESSION = 'none' } = env;
  
  // 通过环境变量控制是否输出 Sass 警告
  process.env.SASS_SILENCE_DEPRECATION_WARNINGS = 'true';
  
  return {
    plugins: [
      // 忽略 Sass 警告信息 - 放在最前面确保其他插件不会输出警告
      silenceSassWarnings(),
      vue(),
      AutoImport({
        resolvers: [ElementPlusResolver()],
        imports: ['vue', 'vue-router'],
        dts: 'src/auto-imports.d.ts',
      }),
      Components({
        resolvers: [ElementPlusResolver()],
        dts: 'src/components.d.ts',
      }),
      // 添加构建信息插件
      viteBuildInfo(),
      // 根据环境变量决定是否添加压缩插件
      configCompressPlugin(VITE_COMPRESSION),
      // 或者直接使用插件列表
      // ...getPluginsList(VITE_CDN === 'true', VITE_COMPRESSION),
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, './src')
      }
    },
    server: {
      host: '0.0.0.0',
      port: 7091,  // 前端端口
      proxy: {
        '/api': {
          target: env.VITE_APP_BASE_URL,  // 从环境变量读取目标地址
          changeOrigin: true,
          secure: false,
          ws: true,
          configure: (proxy, options) => {
            proxy.on('proxyReq', (proxyReq, req, res) => {
              // 只在调试时输出
              if (mode === 'development') {
                console.log('代理请求:', req.method, req.url, '->',
                  options.target + proxyReq.path)
              }
            })
          }
        }
      }
    },
    build: {
      // 输出目录
      outDir: 'dist',
      // 启用/禁用 CSS 代码拆分
      cssCodeSplit: true,
      // 构建后是否生成 source map 文件
      sourcemap: false,
      // 设置最终构建的浏览器兼容目标
      target: 'es2015',
      // 构建后的文件大小警告的限制 (kb)
      chunkSizeWarningLimit: 2000,
      // 启用/禁用 gzip 压缩大小报告
      reportCompressedSize: false,
      // 静默警告
      terserOptions: {
        compress: {
          drop_console: true, // 移除控制台输出
          drop_debugger: true // 移除调试器
        }
      }
    },
    // 添加 CSS 相关配置，禁止 Sass 警告输出
    css: {
      preprocessorOptions: {
        scss: {
          quietDeps: true, // 禁止 Sass 警告输出
          additionalData: fs.existsSync(path.resolve(__dirname, 'src/styles/variables.scss')) 
            ? `@import "@/styles/variables.scss";` 
            : '',
          sassOptions: {
            outputStyle: 'expanded',
            quiet: true, // 安静模式
            logger: { warn: function() {} }, // 空警告函数
            quietDeps: true,
            verbose: false
          }
        }
      },
      // 自定义 Vite 的日志级别，隐藏某些警告
      devSourcemap: false, // 生产环境不需要源映射
      // 忽略 Sass 警告
      postcss: {
        // 可以添加自定义 postcss 插件，这里留空
      }
    },
    // 修改日志级别，隐藏某些警告
    logLevel: 'error', // 或 'warn'
    // 自定义 clearScreen 行为
    clearScreen: true,
    // 环境变量
    define: {
      'process.env.SASS_SILENCE_DEPRECATION_WARNINGS': JSON.stringify('true'),
      'process.env.NODE_ENV': JSON.stringify(mode)
    }
  }
})