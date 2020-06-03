import antd from 'ant-design-vue/es/locale-provider/zh_CN'
import momentCN from 'moment/locale/zh-cn'

const components = {
  antLocale: antd,
  momentName: 'zh-cn',
  momentLocale: momentCN
}

const locale = {
  'message': '-',
  'menu.home': '主页',
  'menu.dashboard': '仪表盘',
  'menu.dashboard.analysis': '分析页',
  'menu.dashboard.monitor': '监控页',
  'menu.dashboard.workplace': '工作台',

  'layouts.usermenu.dialog.title': '消息',
  'layouts.usermenu.dialog.content': 'Do you really log-out.',

  'app.setting.pagestyle': '整体风格设置',
  'app.setting.pagestyle.light': '亮色',
  'app.setting.pagestyle.dark': '暗色',
  'app.setting.pagestyle.realdark': '深色',
  'app.setting.themecolor': '主题色',
  'app.setting.navigationmode': '导航模式',
  'app.setting.content-width': '内容区域宽度',
  'app.setting.fixedheader': '固定顶部',
  'app.setting.fixedsidebar': '固定边栏',
  'app.setting.sidemenu': '侧边菜单布局',
  'app.setting.topmenu': '顶部菜单布局',
  'app.setting.content-width.fixed': '固定',
  'app.setting.content-width.fluid': '流式',
  'app.setting.othersettings': '其他设置',
  'app.setting.weakmode': '色弱模式',
  'app.setting.copy': '复制设置',
  'app.setting.loading': '加载主题',
  'app.setting.copyinfo': '复制设置成功，请替换默认设置在 src/models/setting.js',
  'app.setting.production.hint': '配置栏只在开发环境用于预览，生产环境不会展现，请拷贝后手动修改配置文件'
}

export default {
  ...components,
  ...locale
}
