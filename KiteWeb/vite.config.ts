import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'
import { viteBuildInfo } from './build/info'
import { configCompressPlugin } from './build/compress'
import { silenceSassWarnings } from './build/silence-sass-warnings'
import fs from 'fs'

// 注意: 如需完整功能，请安装以下依赖:
// npm install --save-dev dayjs chalk

// 全局设置环境变量抑制 Sass 警告
process.env.SASS_SILENCE_DEPRECATION_WARNINGS = 'true'
process.env.SASS_SILENCE_DEPRECATION_WARNINGS_DURING_COMPILATION = 'true'
process.env.NODE_ENV_FOR_SASS_WARNINGS = 'production'

export default defineConfig(({ mode, command }) => {
  const env = loadEnv(mode, process.cwd())
  const { VITE_CDN = false, VITE_COMPRESSION = 'none' } = env
  
  return {
    plugins: [
      // 注意: 插件顺序很重要！
      silenceSassWarnings(), // 必须是第一个插件
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
      viteBuildInfo(),
      configCompressPlugin(VITE_COMPRESSION),
    ],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, './src')
      }
    },
    server: {
      host: '0.0.0.0',
      port: 7091,
      open: false, 
      strictPort: false,
      proxy: {
        '/api': {
          target: env.VITE_APP_BASE_URL,
          changeOrigin: true,
          secure: false,
          ws: true,
          configure: (proxy, options) => {
            proxy.on('proxyReq', (proxyReq, req, res) => {
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
      outDir: 'dist',
      cssCodeSplit: true,
      sourcemap: false,
      target: 'es2015',
      chunkSizeWarningLimit: 2000,
      reportCompressedSize: false,
      terserOptions: {
        compress: {
          drop_console: true,
          drop_debugger: true
        }
      }
    },
    css: {
      preprocessorOptions: {
        scss: {
          quietDeps: true,
          additionalData: fs.existsSync(path.resolve(__dirname, 'src/styles/variables.scss'))
            ? `@import "@/styles/variables.scss";`
            : '',
          sassOptions: {
            outputStyle: 'expanded',
            quiet: true,
            logger: { 
              warn: () => {}, 
              debug: () => {},
              info: () => {},
              error: () => {}
            },
            quietDeps: true,
            verbose: false,
            sourceComments: false,
            alertColor: false,
            alertAscii: false,
            omitSourceMapUrl: true,
            quietRuntimeErrors: true,
            quietDependencies: true
          }
        }
      },
      devSourcemap: false,
      postcss: {
        plugins: []
      }
    },
    logLevel: 'info',
    clearScreen: true,
    define: {
      __VUE_PROD_DEVTOOLS__: false,
      'process.env.SASS_SILENCE_DEPRECATION_WARNINGS': JSON.stringify('true'),
      'process.env.NODE_ENV': JSON.stringify(mode),
      'process.env.SASS_PATH': JSON.stringify(''),
      'process.env.DEBUG': JSON.stringify(''),
      'process.env.SASS_SILENCE_DEPRECATION_WARNINGS_DURING_COMPILATION': JSON.stringify('true')
    },
    esbuild: {
      logLevel: 'error',
      logLimit: 0,
      legalComments: 'none'
    }
  }
})