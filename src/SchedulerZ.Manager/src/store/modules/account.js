import Vue from 'vue'
import { login, getInfo, logout } from '@/api/account'
import { ACCESS_TOKEN, REFRESH_TOKEN } from '@/store/mutation-types'
import { welcome } from '@/utils/util'

const account = {
  state: {
    token: '',
    refreshToken: '',
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
    SET_REFRESH_TOKEN: (state, refreshToken) => {
      state.refreshToken = refreshToken
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
          Vue.ls.set(ACCESS_TOKEN, result.accessToken, 7 * 24 * 60 * 60 * 1000)
          commit('SET_TOKEN', result.accessToken)
          Vue.ls.set(REFRESH_TOKEN, result.refreshToken, 7 * 24 * 60 * 60 * 1000)
          commit('SET_REFRESH_TOKEN', result.refreshToken)
          resolve()
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
          commit('SET_ROLES', [])
          Vue.ls.remove(ACCESS_TOKEN)
        })
      })
    },

    // 获取用户信息
    GetInfo ({ commit }) {
      return new Promise((resolve, reject) => {
        getInfo().then(response => {
          const result = response.data

          if (result.roles && result.roles.length > 0) {
            const roles = result.roles
            roles.permissionList = []
            for (let i = 0; i < roles.length; i++) {
              for (let j = 0; j < roles[i].routers.length; j++) {
                const permission = roles[i].routers[j].permission
                if (!roles.permissionList.includes(permission)) {
                  roles.permissionList.push(permission)
                }
              }
            }
            // roles.permissionList = roles[0].routers.map(router => { return router.permission })
            commit('SET_ROLES', result.roles)
            commit('SET_INFO', result.user)
          } else {
            reject(new Error('getInfo: roles must be a non-null array !'))
          }
          console.log(result)
          commit('SET_NAME', { name: result.user.name, welcome: welcome() })
          commit('SET_AVATAR', result.user.avatar)

          resolve(response)
        }).catch(error => {
          reject(error)
        })
      })
    }
  }
}

export default account
