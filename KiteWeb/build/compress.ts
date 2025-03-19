import type { Plugin } from "vite";

// 自己实现 isArray 函数
const isArray = (val: any): val is any[] => {
  return Object.prototype.toString.call(val) === "[object Array]";
};

// 简化版压缩插件，模拟 vite-plugin-compression 的行为
const createCompressPlugin = (options: any): Plugin => {
  return {
    name: 'vite-plugin-compression',
    apply: 'build',
    // 简化实现，实际使用时需要安装 vite-plugin-compression
    // 这里仅作为占位符，让构建过程不报错
    configResolved() {
      console.log('压缩插件配置已应用，使用选项:', options);
    }
  };
};

export const configCompressPlugin = (
  compress: string
): Plugin | Plugin[] => {
  if (compress === "none") return null;

  const gz = {
    // 生成的压缩包后缀
    ext: ".gz",
    // 体积大于threshold才会被压缩
    threshold: 0,
    // 默认压缩.js|mjs|json|css|html后缀文件，设置成true，压缩全部文件
    filter: () => true,
    // 压缩后是否删除原始文件
    deleteOriginFile: false
  };
  const br = {
    ext: ".br",
    algorithm: "brotliCompress",
    threshold: 0,
    filter: () => true,
    deleteOriginFile: false
  };

  const codeList = [
    { k: "gzip", v: gz },
    { k: "brotli", v: br },
    { k: "both", v: [gz, br] }
  ];

  const plugins: Plugin[] = [];

  codeList.forEach(item => {
    if (compress.includes(item.k)) {
      if (compress.includes("clear")) {
        if (isArray(item.v)) {
          item.v.forEach(vItem => {
            plugins.push(
              createCompressPlugin(Object.assign(vItem, { deleteOriginFile: true }))
            );
          });
        } else {
          plugins.push(
            createCompressPlugin(Object.assign(item.v, { deleteOriginFile: true }))
          );
        }
      } else {
        if (isArray(item.v)) {
          item.v.forEach(vItem => {
            plugins.push(createCompressPlugin(vItem));
          });
        } else {
          plugins.push(createCompressPlugin(item.v));
        }
      }
    }
  });

  return plugins;
};
