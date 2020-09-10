import axios from 'axios'
import store from '@/store'
import storage from 'store'
import notification from 'ant-design-vue/es/notification'
import { VueAxios } from './axios'
import { ACCESS_TOKEN, REFRESH_TOKEN } from '@/store/mutation-types'

// 创建 axios 实例
const request = axios.create({
  // API 请求的默认前缀
  baseURL: process.env.VUE_APP_API_BASE_URL,
  timeout: 6000 // 请求超时时间
})

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
      const refreshToken = storage.get(REFRESH_TOKEN)
      if (refreshToken) {
        store.dispatch('Logout').then(() => {
          setTimeout(() => {
            window.location.reload()
          }, 1500)
        })
      }
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
    // 自定义错误
    if (response.data.code === 0) {
      notification.error({
         message: '错误',
         description: response.data.message
       })
     }
     // 参数错误
     if (response.data.code === 10001) {
       notification.warning({
          message: '提示',
          description: response.data.message
        })
      }
     // 服务器异常
     if (response.data.code === 10000) {
       notification.error({
          message: '错误',
          description: response.data.message
        })
      }
     return response.data
}, errorHandler)

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
