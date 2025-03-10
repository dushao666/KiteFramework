declare module 'pinia-plugin-persistedstate' {
  import { PiniaPluginContext } from 'pinia'
  const piniaPluginPersistedstate: (context: PiniaPluginContext) => void
  export default piniaPluginPersistedstate
} 