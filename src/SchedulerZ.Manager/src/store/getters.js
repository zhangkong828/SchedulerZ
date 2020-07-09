const getters = {
  device: state => state.app.device,
  theme: state => state.app.theme,
  color: state => state.app.color,
  token: state => state.account.token,
  avatar: state => state.account.avatar,
  nickname: state => state.account.name,
  welcome: state => state.account.welcome,
  roles: state => state.account.roles,
  userInfo: state => state.account.info,
  addRouters: state => state.permission.addRouters,
  multiTab: state => state.app.multiTab,
  lang: state => state.i18n.lang
}

export default getters
