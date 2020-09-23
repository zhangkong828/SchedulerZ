import axios from 'axios'
import store from '@/store'
import storage from 'store'
import notification from 'ant-design-vue/es/notification'
import { VueAxios } from './axios'
import { ACCESS_TOKEN, ACCESS_TOKEN_EXPIRES, REFRESH_TOKEN, REFRESH_TOKEN_EXPIRES } from '@/store/mutation-types'

const loginRoutePath = '/account/login'

// 创建 axios 实例
const request = axios.create({
  // API 请求的默认前缀
  baseURL: process.env.VUE_APP_API_BASE_URL,
  timeout: 6000 // 请求超时时间
})

request.setToken = (data) => {
  storage.set(ACCESS_TOKEN, data.accessToken)
  store.commit('SET_TOKEN', data.accessToken)
  storage.set(ACCESS_TOKEN_EXPIRES, data.accessTokenExpires)
  store.commit('SET_TOKEN_EXPIRES', data.accessTokenExpires)

  storage.set(REFRESH_TOKEN, data.refreshToken)
  store.commit('SET_REFRESH_TOKEN', data.refreshToken)
  storage.set(REFRESH_TOKEN_EXPIRES, data.refreshTokenExpires)
  store.commit('SET_REFRESH_TOKEN_EXPIRES', data.refreshTokenExpires)
}

// 异常拦截处理器
const errorHandler = (error) => {
  if (error.response) {
    const data = error.response.data
    if (error.response.status === 403) {
      notification.error({
        message: 'Forbidden',
        description: data.message
      })
    }

    if (error.response.status === 401) {
      console.log(error.response)
      const token = storage.get(ACCESS_TOKEN) || ''
      const refreshToken = storage.get(REFRESH_TOKEN) || ''
      if (token && refreshToken) {
        return refreshTokenFun({ AccessToken: token, RefreshToken: refreshToken }).then(response => {
          console.log(response)
          request.setToken(response)
          const config = error.response.config
          config.headers['authorization'] = 'Bearer ' + response.accessToken
          return request(config)
        }).catch(error => {
          console.error('refreshToken error', error)
          window.location.href = loginRoutePath
        })
      }
      window.location.href = loginRoutePath
      // if (refreshToken) {
      //   store.dispatch('Logout').then(() => {
      //     setTimeout(() => {
      //       window.location.reload()
      //     }, 1500)
      //   })
      // }
    }
  }

  console.error('response error', error)
  return Promise.reject(error)
}

// request interceptor
request.interceptors.request.use(config => {
  const token = storage.get(ACCESS_TOKEN)
  if (token) {
    config.headers['authorization'] = 'Bearer ' + token
  }
  return config
}, errorHandler)

// response interceptor
request.interceptors.response.use((response) => {
    if (response.data.code === 1) {
      return response.data
    }

    if (response.data.code === 0) {
      // 自定义错误
      notification.error({
         message: '错误',
         description: response.data.message
       })
     } else if (response.data.code === 10001) {
      // 参数错误
       notification.warning({
          message: '提示',
          description: response.data.message
        })
      } else if (response.data.code === 10000) {
      // 服务器异常
       notification.error({
          message: '服务器错误',
          description: response.data.message
        })
    } else if (response.data.code === 10003) {
      // token刷新失败
      return response.data
    } else {
      // 未定义异常
      notification.error({
        message: '异常',
        description: response.data.message
      })
    }

    return Promise.reject(response.data)
}, errorHandler)

function refreshTokenFun (parameter) {
  return request.post('/account/RefreshToken', parameter).then(response => {
    if (response.code === 10003) {
      return Promise.reject(response)
    } else {
      return response.data
    }
  })
}

const installer = {
  vm: {},
  install (Vue) {
    Vue.use(VueAxios, request)
  }
}

export default request

export {
  installer as VueAxios,
  request as axios
}
