/**
 * 这个文件用于抑制 Sass 的废弃警告
 * 使用 CommonJS 格式，以便与 --require 选项兼容
 */

// 保存原始的控制台方法
const originalConsoleWarn = console.warn;
const originalConsoleError = console.error;

// 设置环境变量来抑制 Sass 警告
process.env.SASS_SILENCE_DEPRECATION_WARNINGS = 'true';
process.env.SASS_SILENCE_DEPRECATION_WARNINGS_DURING_COMPILATION = 'true';
process.env.NODE_ENV_FOR_SASS_WARNINGS = 'production';

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
const isSassWarning = (msg) => {
  if (typeof msg !== 'string') return false;
  try {
    return sassWarningPatterns.some(pattern => 
      msg.toLowerCase().includes(pattern.toLowerCase())
    );
  } catch (e) {
    return false;
  }
};

// 替换全局 console.warn
console.warn = function(...args) {
  if (args.length === 0) return;
  if (typeof args[0] === 'string' && isSassWarning(args[0])) {
    return; // 过滤掉 Sass 警告
  }
  originalConsoleWarn.apply(console, args);
};

// 也替换 console.error 以捕获可能的 Sass 错误警告
console.error = function(...args) {
  if (args.length === 0) return;
  if (typeof args[0] === 'string' && isSassWarning(args[0])) {
    return; // 过滤掉 Sass 错误
  }
  originalConsoleError.apply(console, args);
};

// 覆盖 process.stderr.write 来捕获直接写入的警告
const originalStderrWrite = process.stderr.write;
process.stderr.write = function(chunk, ...args) {
  if (typeof chunk === 'string' && isSassWarning(chunk)) {
    return true; // 过滤掉 Sass 警告
  }
  return originalStderrWrite.apply(process.stderr, [chunk, ...args]);
};

// 在进程退出时恢复原始方法
process.on('exit', () => {
  console.warn = originalConsoleWarn;
  console.error = originalConsoleError;
  process.stderr.write = originalStderrWrite;
});

// 覆盖 Node.js 的 `emitWarning` 方法以捕获 Sass 相关警告
const originalEmitWarning = process.emitWarning;
process.emitWarning = function(warning, ...args) {
  if (warning && typeof warning === 'string' && isSassWarning(warning)) {
    return; // 过滤掉 Sass 警告
  }
  return originalEmitWarning.call(process, warning, ...args);
};

// 让开发者知道这个脚本已加载
console.log("\x1b[36m%s\x1b[0m", "▶ Sass警告抑制器已启用");

// 导出一个空对象，这样当被require时不会有问题
module.exports = {}; 