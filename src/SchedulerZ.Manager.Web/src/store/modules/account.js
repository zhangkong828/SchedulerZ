import storage from 'store'
import { login, getInfo, logout } from '@/api/account'
import { ACCESS_TOKEN, ACCESS_TOKEN_EXPIRES, REFRESH_TOKEN, REFRESH_TOKEN_EXPIRES } from '@/store/mutation-types'
import { welcome } from '@/utils/util'

const user = {
  state: {
    token: '',
    tokenExpires: 0,
    refreshToken: '',
    refreshTokenExpires: 0,
    name: '',
    welcome: '',
    avatar: '',
    roles: [],
    info: {}
  },

  mutations: {
    SET_TOKEN: (state, token) => {
      state.token = token
    },
    SET_TOKEN_EXPIRES: (state, expires) => {
      state.tokenExpires = expires
    },
    SET_REFRESH_TOKEN: (state, refreshToken) => {
      state.refreshToken = refreshToken
    },
    SET_REFRESH_TOKEN_EXPIRES: (state, expires) => {
      state.refreshTokenExpires = expires
    },
    SET_NAME: (state, { name, welcome }) => {
      state.name = name
      state.welcome = welcome
    },
    SET_AVATAR: (state, avatar) => {
      state.avatar = avatar
    },
    SET_ROLES: (state, roles) => {
      state.roles = roles
    },
    SET_INFO: (state, info) => {
      state.info = info
    }
  },

  actions: {
    // 登录
    Login ({ commit }, userInfo) {
      return new Promise((resolve, reject) => {
        login(userInfo).then(response => {
          const result = response.data
          storage.set(ACCESS_TOKEN, result.accessToken)
          commit('SET_TOKEN', result.accessToken)
          storage.set(ACCESS_TOKEN_EXPIRES, result.accessTokenExpires)
          commit('SET_TOKEN_EXPIRES', result.accessTokenExpires)

          storage.set(REFRESH_TOKEN, result.refreshToken)
          commit('SET_REFRESH_TOKEN', result.refreshToken)
          storage.set(REFRESH_TOKEN_EXPIRES, result.refreshTokenExpires)
          commit('SET_REFRESH_TOKEN_EXPIRES', result.refreshTokenExpires)
          resolve(result)
        }).catch(error => {
          reject(error)
        })
      })
    },

    // 获取用户信息
    GetInfo ({ commit }) {
      return new Promise((resolve, reject) => {
        getInfo().then(response => {
          const result = response.data

          if (result.roles && result.roles.length > 0) {
            commit('SET_ROLES', result.roles)
            commit('SET_INFO', result.user)
          } else {
            reject(new Error('getInfo: roles must be a non-null array !'))
          }

          commit('SET_NAME', { name: result.user.name, welcome: welcome() })
          commit('SET_AVATAR', result.user.avatar)

          resolve(response)
        }).catch(error => {
          reject(error)
        })
      })
    },

    // 登出
    Logout ({ commit, state }) {
      return new Promise((resolve) => {
        logout(state.token).then(() => {
          resolve()
        }).catch(() => {
          resolve()
        }).finally(() => {
          commit('SET_TOKEN', '')
          commit('SET_REFRESH_TOKEN', '')
          commit('SET_ROLES', [])
          storage.remove(ACCESS_TOKEN)
          storage.remove(REFRESH_TOKEN)
        })
      })
    }

  }
}

export default user
