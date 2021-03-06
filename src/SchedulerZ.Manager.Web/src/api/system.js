import request from '@/utils/request'

export function getUserList (parameter) {
  return request({
    url: '/system/queryUserList',
    method: 'post',
    data: parameter
  })
}

export function modifyUser (parameter) {
  return request({
    url: '/system/ModifyUser',
    method: 'post',
    data: parameter
  })
}

export function deleteUser (parameter) {
  return request({
    url: '/system/deleteUser',
    method: 'post',
    params: { id: parameter }
  })
}

export function getRoleList (parameter) {
  return request({
    url: '/system/queryRoleList',
    method: 'post',
    data: parameter
  })
}

export function modifyRole (parameter) {
  return request({
    url: '/system/ModifyRole',
    method: 'post',
    data: parameter
  })
}

export function deleteRole (parameter) {
  return request({
    url: '/system/deleteRole',
    method: 'post',
    params: { id: parameter }
  })
}

export function getPermissionTree (parameter) {
  return request({
    url: '/system/queryPermissionTreeList',
    method: 'post',
    data: parameter
  })
}

export function getPermissions (parameter) {
  return request({
    url: '/system/queryPermissionList',
    method: 'post',
    data: parameter
  })
}

export function modifyPermission (parameter) {
  return request({
    url: '/system/modifyPermission',
    method: 'post',
    data: parameter
  })
}

export function deletePermission (parameter) {
  return request({
    url: '/system/deletePermission',
    method: 'post',
    params: { id: parameter }
  })
}
