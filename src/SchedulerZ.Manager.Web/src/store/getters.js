const getters = {
  isMobile: state => state.app.isMobile,
  lang: state => state.app.lang,
  theme: state => state.app.theme,
  color: state => state.app.color,
  token: state => state.account.token,
  avatar: state => state.account.avatar,
  nickname: state => state.account.name,
  welcome: state => state.account.welcome,
  roles: state => state.account.roles,
  userInfo: state => state.account.info,
  addRouters: state => state.permission.addRouters,
  multiTab: state => state.app.multiTab
}

export default getters
