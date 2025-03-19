export interface ViteEnv {
  VITE_PORT: number;
  VITE_PUBLIC_PATH: string;
  VITE_ROUTER_HISTORY: string;
  VITE_CDN: boolean;
  VITE_HIDE_HOME: string;
  VITE_COMPRESSION: string;
  VITE_APP_BASE_API: string;
  VITE_APP_BASE_URL: string;
  VITE_APP_BASE_URL_WS: string;
  VITE_APP_BASE_WS: string;
  VITE_CORP_ID: string;
  VITE_CLIENT_ID: string;
}

export type Recordable<T = any> = Record<string, T>; 