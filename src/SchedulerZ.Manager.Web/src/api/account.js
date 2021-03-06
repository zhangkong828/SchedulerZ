import request from '@/utils/request'

/**
 * login func
 * parameter: {
 *     username: '',
 *     password: '',
 *     remember_me: true,
 *     captcha: '12345'
 * }
 * @param parameter
 * @returns {*}
 */
export function login (parameter) {
  return request({
    url: '/account/login',
    method: 'post',
    data: parameter
  })
}

export function getInfo () {
  return request({
    url: '/account/info',
    method: 'get'
  })
}

export function getCurrentUserNav () {
  return request({
    url: '/account/GetUserNav',
    method: 'get'
  })
}

export function logout () {
  return request({
    url: '/account/logout',
    method: 'post',
    headers: {
      'Content-Type': 'application/json;charset=UTF-8'
    }
  })
}

/**
 * get user 2step code open?
 * @param parameter {*}
 */
export function get2step (parameter) {
  return request({
    url: '',
    method: 'post',
    data: parameter
  })
}

export function getUserList (parameter) {
  return request({
    url: '/account/getUserList',
    method: 'get',
    params: parameter
  })
}
