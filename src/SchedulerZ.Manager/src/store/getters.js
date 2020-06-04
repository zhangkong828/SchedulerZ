const getters = {
  sideCollapsed: state => state.app.sideCollapsed,
  isMobile: state => state.app.isMobile,
  theme: state => state.app.theme,
  layout: state => state.app.layout,
  contentWidth: state => state.app.contentWidth,
  fixedHeader: state => state.app.fixedHeader,
  fixedSidebar: state => state.app.fixedSidebar,
  autoHideHeader: state => state.app.autoHideHeader,
  color: state => state.app.color,
  weak: state => state.app.weak,
  multiTab: state => state.app.multiTab,
  lang: state => state.app.lang,
  token: state => state.user.token,
  avatar: state => state.user.avatar,
  nickname: state => state.user.name,
  welcome: state => state.user.welcome,
  roles: state => state.user.roles,
  userInfo: state => state.user.info,
  addRouters: state => state.permission.addRouters
}

export default getters
