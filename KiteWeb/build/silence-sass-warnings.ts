import type { Plugin } from 'vite';

/**
 * 忽略 Sass 警告的 Vite 插件
 * 通过拦截 Vite 的警告处理，过滤掉 Sass 相关的警告消息
 */
export function silenceSassWarnings(): Plugin {
  // 要过滤的警告消息关键词列表
  const sassWarningPatterns = [
    'Deprecation Warning [legacy-js-api]',
    'More info: https://sass-lang.com/d/legacy-js-api',
    'sass-lang.com',
    'legacy-js-api',
  ];
  
  // 检查是否应该过滤特定消息
  const shouldFilter = (msg: string): boolean => {
    return sassWarningPatterns.some(pattern => msg.includes(pattern));
  };
  
  // 钩子出现的上下文
  const warnFilter = (msg: string, options: any): boolean => {
    return !shouldFilter(msg);
  };
  
  return {
    name: 'silence-sass-warnings',
    enforce: 'pre',
    // 启动时应用过滤器
    buildStart() {
      // 替换全局 console.warn
      const originalWarn = console.warn;
      console.warn = function(msg, ...args) {
        if (typeof msg === 'string' && shouldFilter(msg)) {
          return;
        }
        originalWarn.call(console, msg, ...args);
      };
    },
    // 热更新时应用过滤器
    handleHotUpdate({ server }) {
      const originalWarn = server.config.logger.warn;
      server.config.logger.warn = (msg, options) => {
        if (typeof msg === 'string' && shouldFilter(msg)) {
          return;
        }
        originalWarn(msg, options);
      };
    },
    // 配置服务器时应用过滤器
    configureServer(server) {
      const originalWarn = server.config.logger.warn;
      server.config.logger.warn = (msg, options) => {
        if (typeof msg === 'string' && shouldFilter(msg)) {
          return;
        }
        originalWarn(msg, options);
      };
    },
    // 转换阶段应用过滤器
    transform(code, id) {
      if (id.endsWith('.scss') || id.endsWith('.sass')) {
        // 针对 Sass 文件的转换时，捕获可能的警告
        return {
          code,
          map: null
        };
      }
      return null;
    }
  };
} 