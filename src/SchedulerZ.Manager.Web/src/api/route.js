import request from '@/utils/request'

export function getNodeList (parameter) {
  return request({
    url: '/Route/NodeList',
    method: 'post',
    data: parameter
  })
}

export function getServiceList (parameter) {
  return request({
    url: '/Route/ServiceList',
    method: 'post',
    data: parameter
  })
}
