import { defineStore } from 'pinia'

interface UserInfo {
  username: string
  userid: string
  userInfo: any
  accessToken: string
  refreshToken: string
}

export const useUserStore = defineStore('user', {
  persist: {
    key: 'userState',
    storage: localStorage,
  },

  state: () => ({
    isLoggedIn: false,
    username: '',
    userid: '',
    jwtAccessToken: null as string | null,
    jwtRefreshToken: null as string | null,
    userInfo: null as any,
  }),

  actions: {
    setUserInfo(data: UserInfo) {
      if (!data) return Promise.reject('No user data provided')
      try {
        this.username = data.username
        this.userid = data.userid
        this.isLoggedIn = true
        this.userInfo = data.userInfo
        this.jwtAccessToken = data.accessToken
        this.jwtRefreshToken = data.refreshToken
        return Promise.resolve(true)
      } catch (error) {
        return Promise.reject(error)
      }
    },

    logout() {
      this.isLoggedIn = false
      this.username = ''
      this.userid = ''
      this.jwtAccessToken = null
      this.jwtRefreshToken = null
      this.userInfo = null
      localStorage.removeItem('userState')
    },
  },

  getters: {
    getIsLoggedIn: (state) => state.isLoggedIn,
    getUsername: (state) => state.username,
    getUserid: (state) => state.userid,
    getUserAccessToken: (state) => state.jwtAccessToken,
    getRefreshToken: (state) => state.jwtRefreshToken,
  },
}) 