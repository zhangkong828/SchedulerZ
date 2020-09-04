import request from '@/utils/request'

export function getUserList (parameter) {
  return request({
    url: '/system/getUserList',
    method: 'post',
    data: parameter
  })
}

export function getRoleList (parameter) {
  return request({
    url: '/system/getUserList',
    method: 'post',
    params: parameter
  })
}

export function getPermissions (parameter) {
  return request({
    url: '/system/getUserList',
    method: 'post',
    params: parameter
  })
}
