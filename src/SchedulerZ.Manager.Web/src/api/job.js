import request from '@/utils/request'

export function uploadPackage (parameter) {
  return request({
    url: '/Packages/UploadPackage',
    method: 'post',
    headers: { 'Content-Type': 'multipart/form-data' },
    data: parameter,
    timeout: 60000
  })
}

export function getJobList (parameter) {
  return request({
    url: '/Job/JobList',
    method: 'post',
    data: parameter
  })
}

export function modifyJob (parameter) {
  return request({
    url: '/Job/ModifyJob',
    method: 'post',
    data: parameter
  })
}

export function startJob (parameter) {
  return request({
    url: '/Job/StartJob',
    method: 'post',
    params: { id: parameter }
  })
}

export function runOnceNowJob (parameter) {
  return request({
    url: '/Job/RunOnceNowJob',
    method: 'post',
    params: { id: parameter }
  })
}

export function pauseJob (parameter) {
  return request({
    url: '/Job/PauseJob',
    method: 'post',
    params: { id: parameter }
  })
}

export function resumeJob (parameter) {
  return request({
    url: '/Job/ResumeJob',
    method: 'post',
    params: { id: parameter }
  })
}

export function stopJob (parameter) {
  return request({
    url: '/Job/StopJob',
    method: 'post',
    params: { id: parameter }
  })
}

export function deleteJob (parameter) {
  return request({
    url: '/Job/DeleteJob',
    method: 'post',
    params: { id: parameter }
  })
}
