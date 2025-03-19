import type { Plugin } from "vite";
import { getPackageSize } from "./utils";
import dayjs, { type Dayjs } from "dayjs";
import duration from "dayjs/plugin/duration";
dayjs.extend(duration);

// 记录开始时间 - 使用精确时间戳
const startTime = Date.now();

// 防止欢迎信息重复显示的标志
// 使用全局对象以避免热重载问题
if (typeof globalThis.__kiteFrameworkWelcomeDisplayed === 'undefined') {
  globalThis.__kiteFrameworkWelcomeDisplayed = false;
}

// 定义颜色代码
const colors = {
  black: '\x1b[30m',
  red: '\x1b[31m',
  green: '\x1b[32m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m',
  magenta: '\x1b[35m',
  cyan: '\x1b[36m',
  white: '\x1b[37m',
  reset: '\x1b[0m',
  // 背景色
  bgBlack: '\x1b[40m',
  bgRed: '\x1b[41m',
  bgGreen: '\x1b[42m',
  bgYellow: '\x1b[43m',
  bgBlue: '\x1b[44m',
  bgMagenta: '\x1b[45m',
  bgCyan: '\x1b[46m',
  bgWhite: '\x1b[47m',
  // 明亮版背景色
  bgBrightBlack: '\x1b[100m',
  // 样式
  bright: '\x1b[1m',
  dim: '\x1b[2m',
  italic: '\x1b[3m',
  underline: '\x1b[4m'
};

// 计算文本宽度（考虑中文和特殊字符）
function getStringWidth(str: string): number {
  if (!str) return 0;
  
  try {
    let width = 0;
    for (let i = 0; i < str.length; i++) {
      const code = str.codePointAt(i);
      if (!code) continue;
      
      // 如果是emoji或多字节字符（中文等）
      if (code > 0xFF) {
        width += 2; // 中文字符占用2个宽度
        // 如果是代理对（emoji可能是由两个code unit组成）
        if (code > 0xFFFF) {
          i++; // 跳过下一个代码单元
        }
      } else {
        width += 1; // ASCII字符占用1个宽度
      }
    }
    return width;
  } catch (err) {
    // 如果出错，返回字符串长度作为后备方案
    return str.length;
  }
}

// 定义渐变色配置 - 在几种颜色之间平滑过渡
const gradientColors = [
  { r: 66, g: 211, b: 255 },  // 浅蓝色
  { r: 107, g: 126, b: 255 }, // 紫蓝色
  { r: 148, g: 87, b: 255 },  // 紫色
  { r: 249, g: 65, b: 255 },  // 粉紫色
  { r: 66, g: 211, b: 255 }   // 浅蓝色 (循环回来)
];

// 创建平滑渐变色文本 - 一个颜色逐渐过渡到另一个颜色
function createSmoothGradientText(text: string): string {
  // 安全检查
  if (!text) return '';
  
  try {
    // 将字符串分解为字符数组，处理特殊字符
    const chars = [];
    for (let i = 0; i < text.length; i++) {
      const code = text.codePointAt(i);
      if (!code) continue;
      
      if (code > 0xFFFF) {
        chars.push(text.substring(i, i + 2));
        i++; // 跳过下一个代码单元
      } else {
        chars.push(text[i]);
      }
    }
    
    // 使用渐变色为每个字符上色
    let result = '';
    const totalChars = chars.length;
    
    if (totalChars === 0) return text;
    
    chars.forEach((char, index) => {
      // 计算当前渐变位置 (0-1范围)
      const position = index / totalChars;
      // 确定颜色区段
      const segmentCount = gradientColors.length - 1;
      const segment = Math.min(Math.floor(position * segmentCount), segmentCount - 1);
      // 计算区段内位置 (0-1范围)
      const segmentPosition = (position * segmentCount) - segment;
      
      // 获取当前区段的起始和结束颜色
      const startColor = gradientColors[segment];
      const endColor = gradientColors[segment + 1];
      
      // 使用三次方缓动函数使过渡更平滑
      const easeInOut = segmentPosition < 0.5
        ? 4 * segmentPosition * segmentPosition * segmentPosition
        : 1 - Math.pow(-2 * segmentPosition + 2, 3) / 2;
      
      // 计算过渡色
      const r = Math.round(startColor.r + (endColor.r - startColor.r) * easeInOut);
      const g = Math.round(startColor.g + (endColor.g - startColor.g) * easeInOut);
      const b = Math.round(startColor.b + (endColor.b - startColor.b) * easeInOut);
      
      // 使用RGB格式的ANSI转义码
      const colorCode = `\x1b[38;2;${r};${g};${b}m`;
      result += colors.bright + colorCode + char + colors.reset;
    });
    
    return result;
  } catch (err) {
    // 如果发生任何错误，返回原始文本
    console.error('创建渐变文本时出错:', err);
    return text;
  }
}

// 创建欢迎框
function createWelcomeBox(): void {
  try {
    // 确保只显示一次
    if (typeof globalThis.__kiteFrameworkWelcomeDisplayed !== 'undefined' && 
        globalThis.__kiteFrameworkWelcomeDisplayed === true) return;
    globalThis.__kiteFrameworkWelcomeDisplayed = true;
    
    // 捕获标准错误和警告，防止输出
    const originalConsoleError = console.error;
    const originalConsoleWarn = console.warn;
    console.error = () => {};
    console.warn = () => {};
    
    try {
      // 准备文本内容
      const lines = [
        '欢迎使用 Kite Framework',
        '一个轻量级的全栈开发框架',
        'https://github.com/dushao666/KiteFramework'
      ];
      
      // 为每行创建渐变效果
      const gradientLines = lines.map(line => createSmoothGradientText(line));
      
      // 计算最长行的宽度 (不含颜色代码)
      const maxWidth = Math.max(...lines.map(line => getStringWidth(line)));
      const boxWidth = maxWidth + 4;
      
      // 准备样式
      const boxBg = colors.bgBlack;
      const borderColor = colors.cyan;
      
      // 构建边框
      const topBorder = boxBg + borderColor + '┌' + '─'.repeat(boxWidth) + '┐' + colors.reset;
      const bottomBorder = boxBg + borderColor + '└' + '─'.repeat(boxWidth) + '┘' + colors.reset;
      
      // 输出完整框
      console.log('\n' + topBorder);
      gradientLines.forEach((line, index) => {
        // 计算当前行的宽度 (不含颜色代码)
        const lineWidth = getStringWidth(lines[index]);
        // 计算右侧填充
        const rightPadding = boxWidth - lineWidth - 1;
        
        console.log(
          boxBg + borderColor + '│' + 
          ' ' + line + 
          boxBg + ' '.repeat(rightPadding) + 
          borderColor + '│' + colors.reset
        );
      });
      console.log(bottomBorder + '\n');
    } finally {
      // 恢复控制台函数
      console.error = originalConsoleError;
      console.warn = originalConsoleWarn;
    }
  } catch (err) {
    // 如果有任何错误，尝试简单地显示欢迎消息
    console.log('\n欢迎使用 Kite Framework - 一个轻量级的全栈开发框架\n');
  }
}

// 创建构建完成信息框
function createBuildInfoBox(time: string, size: string): void {
  try {
    // 捕获标准错误和警告，防止输出
    const originalConsoleError = console.error;
    const originalConsoleWarn = console.warn;
    console.error = () => {};
    console.warn = () => {};
    
    try {
      // 准备样式
      const boxBg = colors.bgBlack;
      const borderColor = colors.cyan;
      
      // 准备内容
      const content = `🎉 构建完成!\n\n总用时: ${time}\n打包大小: ${size}`;
      const lines = content.split('\n');
      const gradientLines = lines.map(line => createSmoothGradientText(line));
      
      // 计算最长行的宽度 (不含颜色代码)
      const maxWidth = Math.max(...lines.map(line => getStringWidth(line)));
      const boxWidth = maxWidth + 4;
      
      // 构建边框
      const topBorder = boxBg + borderColor + '┌' + '─'.repeat(boxWidth) + '┐' + colors.reset;
      const bottomBorder = boxBg + borderColor + '└' + '─'.repeat(boxWidth) + '┘' + colors.reset;
      
      // 输出完整框
      console.log('\n' + topBorder);
      gradientLines.forEach((line, index) => {
        // 计算当前行的宽度 (不含颜色代码)
        const lineWidth = getStringWidth(lines[index]);
        // 计算右侧填充
        const rightPadding = boxWidth - lineWidth - 1;
        
        console.log(
          boxBg + borderColor + '│' + 
          ' ' + line + 
          boxBg + ' '.repeat(rightPadding) + 
          borderColor + '│' + colors.reset
        );
      });
      console.log(bottomBorder + '\n');
    } finally {
      // 恢复控制台函数
      console.error = originalConsoleError;
      console.warn = originalConsoleWarn;
    }
  } catch (err) {
    // 如果有任何错误，尝试简单地显示构建信息
    console.log(`\n🎉 构建完成! 总用时: ${time}，打包大小: ${size}\n`);
  }
}

// 屏蔽所有控制台警告
const originalConsoleWarn = console.warn;
// 只在预定义的场景下抑制警告，而不是全局重写
// console.warn = function() {};

// 屏蔽所有控制台错误
const originalConsoleError = console.error;
// 只在预定义的场景下抑制错误，而不是全局重写 
// console.error = function() {};

// 显示欢迎信息 - 只显示一次
createWelcomeBox();

// 格式化时间差（毫秒）为分钟和秒
function formatTimeDiff(ms: number): string {
  try {
    const totalSeconds = Math.floor(ms / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes.toString().padStart(2, '0')}分${seconds.toString().padStart(2, '0')}秒`;
  } catch (err) {
    // 出错时返回默认时间
    return '00分00秒';
  }
}

export function viteBuildInfo(): Plugin {
  let config: { command: string };
  let outDir: string;

  return {
    name: "vite:buildInfo",
    configResolved(resolvedConfig) {
      config = resolvedConfig;
      outDir = resolvedConfig.build?.outDir ?? "dist";
    },
    buildStart() {
      // 忽略警告
    },
    closeBundle() {
      if (config.command === "build") {
        getPackageSize({
          folder: outDir,
          callback: (size: string) => {
            // 计算构建时间 - 使用原始时间戳计算差值
            const endTime = Date.now();
            const timeDiff = endTime - startTime;
            const formattedTime = formatTimeDiff(timeDiff);
            
            // 使用与截图完全一致的渐变色输出格式
            createBuildInfoBox(formattedTime, size);
          }
        });
      }
    }
  };
}
