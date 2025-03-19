import type { Plugin } from 'vite';

/**
 * 忽略 Sass 警告的 Vite 插件
 * 彻底拦截和过滤掉 Sass 相关的警告消息
 */
export function silenceSassWarnings(): Plugin {
  // 要过滤的 Sass 警告关键词
  const sassWarningPatterns = [
    'Deprecation Warning', 
    'legacy-js-api',
    'sass-lang.com',
    'The legacy JS API',
    'dart-sass',
    'deprecated',
    'DEPRECATION WARNING',
    'will be removed in Dart Sass 2.0.0'
  ];
  
  // 检查是否是 Sass 警告
  const isSassWarning = (msg: string): boolean => {
    if (typeof msg !== 'string') return false;
    return sassWarningPatterns.some(pattern => 
      msg.toLowerCase().includes(pattern.toLowerCase())
    );
  };
  
  // 保存原始控制台函数
  const originalConsoleWarn = console.warn;
  const originalConsoleError = console.error;
  
  // 全局设置环境变量抑制 Sass 警告
  process.env.SASS_SILENCE_DEPRECATION_WARNINGS = 'true';
  
  return {
    name: 'vite:silence-sass-warnings',
    apply: 'serve', // 仅在开发服务器模式下应用
    
    // 在配置解析后应用
    configResolved(config) {
      // 这里不直接修改 config.logLevel，因为它是只读的
      // 而是改用 process.env 来设置日志级别
      process.env.VITE_LOG_LEVEL = 'error';
    },
    
    // 在构建开始前执行
    buildStart() {
      // 替换全局 console.warn
      console.warn = function(...args: any[]) {
        if (args.length > 0 && isSassWarning(args[0])) {
          return; // 过滤掉 Sass 警告
        }
        originalConsoleWarn.apply(console, args);
      };
      
      // 也替换 console.error 以捕获可能的 Sass 错误警告
      console.error = function(...args: any[]) {
        if (args.length > 0 && isSassWarning(args[0])) {
          return; // 过滤掉 Sass 警告
        }
        originalConsoleError.apply(console, args);
      };
    },
    
    // 在构建结束时恢复控制台函数
    buildEnd() {
      console.warn = originalConsoleWarn;
      console.error = originalConsoleError;
    },
    
    // 处理开发服务器上的热更新
    handleHotUpdate({ server }) {
      if (server && server.config.logger) {
        // 拦截服务器日志警告
        const logger = server.config.logger;
        const originalLoggerWarn = logger.warn;
        
        logger.warn = (msg, options) => {
          if (isSassWarning(msg)) {
            return; // 过滤掉 Sass 警告
          }
          originalLoggerWarn(msg, options);
        };
      }
    },
    
    // 配置开发服务器
    configureServer(server) {
      // 拦截服务器日志警告
      const logger = server.config.logger;
      const originalLoggerWarn = logger.warn;
      
      logger.warn = (msg, options) => {
        if (isSassWarning(msg)) {
          return; // 过滤掉 Sass 警告
        }
        originalLoggerWarn(msg, options);
      };
    }
  };
} 