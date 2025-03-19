import type { Plugin } from "vite";
import { getPackageSize } from "./utils";
import dayjs, { type Dayjs } from "dayjs";
import duration from "dayjs/plugin/duration";
dayjs.extend(duration);

// 记录开始时间 - 使用精确时间戳
const startTime = Date.now();

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
}

// 创建多行欢迎信息框，完全匹配截图风格
function createWelcomeBox(): void {
  // 捕获标准错误，防止输出警告
  const originalConsoleError = console.error;
  const originalConsoleWarn = console.warn;
  console.error = () => {};
  console.warn = () => {};
  
  // 准备文本内容
  const lines = [
    '您好! 欢迎使用 Kite Framework 开源项目',
    '我们为您精心准备了下面的文档',
    'https://github.com/dushaoqi666/KiteFramework'
  ];
  
  // 计算最长行的宽度
  const maxWidth = Math.max(...lines.map(line => getStringWidth(line)));
  const boxWidth = maxWidth + 6; // 添加足够的边距
  
  // 准备样式
  const boxBg = colors.bgBlack;
  const borderColor = colors.cyan;
  const textColor = colors.bright + colors.cyan;
  
  // 构建顶部和底部边框
  const topBorder = boxBg + borderColor + '┌' + '─'.repeat(boxWidth) + '┐' + colors.reset;
  const bottomBorder = boxBg + borderColor + '└' + '─'.repeat(boxWidth) + '┘' + colors.reset;
  
  // 输出顶部边框
  console.log(topBorder);
  
  // 输出内容行
  lines.forEach((line, index) => {
    // 链接行使用不同颜色
    const lineColor = index === 2 ? colors.bright + colors.cyan : textColor;
    
    // 计算当前行填充
    const padding = ' '.repeat(boxWidth - getStringWidth(line));
    
    console.log(
      boxBg + borderColor + '│' + 
      ' ' + lineColor + line + colors.reset + 
      boxBg + padding + 
      borderColor + '│' + colors.reset
    );
  });
  
  // 输出底部边框
  console.log(bottomBorder);
  
  // 恢复控制台函数
  console.error = originalConsoleError;
  console.warn = originalConsoleWarn;
}

// 创建构建完成信息
function createBuildCompleteBox(time: string, size: string): void {
  // 捕获标准错误，防止输出警告
  const originalConsoleError = console.error;
  const originalConsoleWarn = console.warn;
  console.error = () => {};
  console.warn = () => {};
  
  // 准备样式
  const boxBg = colors.bgBlack;
  const borderColor = colors.cyan;
  const textColor = colors.bright + colors.cyan;
  
  // 准备内容
  const content = `🎉 恭喜打包完成（总用时${time}，打包后的大小为${size}）`;
  const contentWidth = getStringWidth(content);
  const boxWidth = contentWidth + 6;
  
  // 构建边框
  const topBorder = boxBg + borderColor + '┌' + '─'.repeat(boxWidth) + '┐' + colors.reset;
  const bottomBorder = boxBg + borderColor + '└' + '─'.repeat(boxWidth) + '┘' + colors.reset;
  
  // 输出框
  console.log('\n' + topBorder);
  console.log(
    boxBg + borderColor + '│' + 
    ' ' + textColor + content + colors.reset + 
    boxBg + ' '.repeat(4) + 
    borderColor + '│' + colors.reset
  );
  console.log(bottomBorder + '\n');
  
  // 恢复控制台函数
  console.error = originalConsoleError;
  console.warn = originalConsoleWarn;
}

// 显示欢迎信息，格式完全匹配截图
createWelcomeBox();

// 格式化时间差（毫秒）为分钟和秒
function formatTimeDiff(ms: number): string {
  const totalSeconds = Math.floor(ms / 1000);
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${minutes.toString().padStart(2, '0')}分${seconds.toString().padStart(2, '0')}秒`;
}

// 屏蔽所有控制台警告
const originalConsoleWarn = console.warn;
console.warn = function() {};

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
            
            // 使用与截图一致的输出格式
            createBuildCompleteBox(formattedTime, size);
          }
        });
      }
    }
  };
}
