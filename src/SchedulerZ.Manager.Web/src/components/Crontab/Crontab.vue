<template>
  <div>
    <a-tabs v-model="defaultActiveTab">
      <a-tab-pane key="1" tab="秒" :forceRender="true">
        <CrontabSecond @update="updateContabValue" :check="checkNumber" ref="cronsecond" />
      </a-tab-pane>

      <a-tab-pane key="2" tab="分钟" :forceRender="true">
        <CrontabMin
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronmin"
        />
      </a-tab-pane>

      <a-tab-pane key="3" tab="小时" :forceRender="true">
        <CrontabHour
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronhour"
        />
      </a-tab-pane>

      <a-tab-pane key="4" tab="日" :forceRender="true">
        <CrontabDay
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronday"
        />
      </a-tab-pane>

      <a-tab-pane key="5" tab="月" :forceRender="true">
        <CrontabMouth
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronmouth"
        />
      </a-tab-pane>

      <a-tab-pane key="6" tab="周" :forceRender="true">
        <CrontabWeek
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronweek"
        />
      </a-tab-pane>

      <a-tab-pane key="7" tab="年" :forceRender="true">
        <CrontabYear
          @update="updateContabValue"
          :check="checkNumber"
          :cron="contabValueObj"
          ref="cronyear"
        />
      </a-tab-pane>
    </a-tabs>

    <div class="popup-main">
      <div class="popup-result">
        <p class="title">时间表达式</p>
        <table>
          <thead>
            <th v-for="item of tabTitles" width="40" :key="item">{{ item }}</th>
            <th>crontab完整表达式</th>
          </thead>
          <tbody>
            <td>
              <span>{{ contabValueObj.second }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.min }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.hour }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.day }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.mouth }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.week }}</span>
            </td>
            <td>
              <span>{{ contabValueObj.year }}</span>
            </td>
            <td>
              <span>{{ contabValueString }}</span>
            </td>
          </tbody>
        </table>
      </div>
      <CrontabResult :ex="contabValueString"></CrontabResult>

      <div class="pop_btn">
        <a-button type="primary" @click="submitFill">确定</a-button>
        <a-button type="warning" @click="clearCron">重置</a-button>
        <a-button @click="hidePopup">取消</a-button>
      </div>
    </div>
  </div>
</template>

<script>
import CrontabSecond from './Crontab-Second.vue'
import CrontabMin from './Crontab-Min.vue'
import CrontabHour from './Crontab-Hour.vue'
import CrontabDay from './Crontab-Day.vue'
import CrontabMouth from './Crontab-Mouth.vue'
import CrontabWeek from './Crontab-Week.vue'
import CrontabYear from './Crontab-Year.vue'
import CrontabResult from './Crontab-Result.vue'

export default {
  name: 'Crontab',
  props: {
    expression: {
      type: String,
      default: '* * * * * ?'
    },
    hideComponent: {
      type: Boolean
    }
  },
  data () {
    return {
      defaultActiveTab: '1',
      tabTitles: ['秒', '分钟', '小时', '日', '月', '周', '年'],
      tabActive: 0,
      myindex: 0,
      contabValueObj: {
        second: '*',
        min: '*',
        hour: '*',
        day: '*',
        mouth: '*',
        week: '?',
        year: ''
      }
    }
  },
  methods: {
    shouldHide (key) {
      if (this.hideComponent && this.hideComponent.includes(key)) return false
      return true
    },
    resolveExp () {
      // 反解析 表达式
      if (this.expression) {
        const arr = this.expression.split(' ')
        if (arr.length >= 6) {
          // 6 位以上是合法表达式
          const obj = {
            second: arr[0],
            min: arr[1],
            hour: arr[2],
            day: arr[3],
            mouth: arr[4],
            week: arr[5],
            year: arr[6] ? arr[6] : ''
          }
          this.contabValueObj = {
            ...obj
          }
          for (var i in obj) {
            if (obj[i]) this.changeRadio(i, obj[i])
          }
        }
      } else {
        // 没有传入的表达式 则还原
        this.clearCron()
      }
    },
    // tab切换值
    tabCheck (index) {
      this.tabActive = index
    },
    // 由子组件触发，更改表达式组成的字段值
    updateContabValue (name, value, from) {
      // "updateContabValue", name, value, from;
      this.contabValueObj[name] = value
      if (from && from !== name) {
        console.log(`来自组件 ${from} 改变了 ${name} ${value}`)
        this.changeRadio(name, value)
      }
    },
    // 赋值到组件
    changeRadio (name, value) {
      var arr = ['second', 'min', 'hour', 'mouth']
      var refName = 'cron' + name
      var insVlaue
      var indexArr
      if (!this.$refs[refName]) return

      if (arr.includes(name)) {
        if (value === '*') {
          insVlaue = 1
        } else if (value.indexOf('-') > -1) {
          indexArr = value.split('-')
          isNaN(indexArr[0])
            ? (this.$refs[refName].cycle01 = 0)
            : (this.$refs[refName].cycle01 = indexArr[0])
          this.$refs[refName].cycle02 = indexArr[1]
          insVlaue = 2
        } else if (value.indexOf('/') > -1) {
          indexArr = value.split('/')
          isNaN(indexArr[0])
            ? (this.$refs[refName].average01 = 0)
            : (this.$refs[refName].average01 = indexArr[0])
          this.$refs[refName].average02 = indexArr[1]
          insVlaue = 3
        } else {
          insVlaue = 4
          this.$refs[refName].checkboxList = value.split(',')
        }
      } else if (name === 'day') {
        if (value === '*') {
          insVlaue = 1
        } else if (value === '?') {
          insVlaue = 2
        } else if (value.indexOf('-') > -1) {
          indexArr = value.split('-')
          isNaN(indexArr[0])
            ? (this.$refs[refName].cycle01 = 0)
            : (this.$refs[refName].cycle01 = indexArr[0])
          this.$refs[refName].cycle02 = indexArr[1]
          insVlaue = 3
        } else if (value.indexOf('/') > -1) {
          indexArr = value.split('/')
          isNaN(indexArr[0])
            ? (this.$refs[refName].average01 = 0)
            : (this.$refs[refName].average01 = indexArr[0])
          this.$refs[refName].average02 = indexArr[1]
          insVlaue = 4
        } else if (value.indexOf('W') > -1) {
          indexArr = value.split('W')
          isNaN(indexArr[0])
            ? (this.$refs[refName].workday = 0)
            : (this.$refs[refName].workday = indexArr[0])
          insVlaue = 5
        } else if (value === 'L') {
          insVlaue = 6
        } else {
          this.$refs[refName].checkboxList = value.split(',')
          insVlaue = 7
        }
      } else if (name === 'week') {
        if (value === '*') {
          insVlaue = 1
        } else if (value === '?') {
          insVlaue = 2
        } else if (value.indexOf('-') > -1) {
          indexArr = value.split('-')
          isNaN(indexArr[0])
            ? (this.$refs[refName].cycle01 = 0)
            : (this.$refs[refName].cycle01 = indexArr[0])
          this.$refs[refName].cycle02 = indexArr[1]
          insVlaue = 3
        } else if (value.indexOf('#') > -1) {
          indexArr = value.split('#')
          isNaN(indexArr[0])
            ? (this.$refs[refName].average01 = 1)
            : (this.$refs[refName].average01 = indexArr[0])
          this.$refs[refName].average02 = indexArr[1]
          insVlaue = 4
        } else if (value.indexOf('L') > -1) {
          indexArr = value.split('L')
          isNaN(indexArr[0])
            ? (this.$refs[refName].weekday = 1)
            : (this.$refs[refName].weekday = indexArr[0])
          insVlaue = 5
        } else {
          this.$refs[refName].checkboxList = value.split(',')
          insVlaue = 7
        }
      } else if (name === 'year') {
        if (value === '') {
          insVlaue = 1
        } else if (value === '*') {
          insVlaue = 2
        } else if (value.indexOf('-') > -1) {
          insVlaue = 3
        } else if (value.indexOf('/') > -1) {
          insVlaue = 4
        } else {
          this.$refs[refName].checkboxList = value.split(',')
          insVlaue = 5
        }
      }
      this.$refs[refName].radioValue = insVlaue
    },
    // 表单选项的子组件校验数字格式（通过-props传递）
    checkNumber (value, minLimit, maxLimit) {
      // 检查必须为整数
      value = Math.floor(value)
      if (value < minLimit) {
        value = minLimit
      } else if (value > maxLimit) {
        value = maxLimit
      }
      return value
    },
    // 隐藏弹窗
    hidePopup () {
      this.defaultActiveTab = '1'
      this.resolveExp()
      this.$emit('hide')
    },
    // 填充表达式
    submitFill () {
      this.$emit('fill', this.contabValueString)
      this.hidePopup()
    },
    clearCron () {
      // 还原选择项
      this.contabValueObj = {
        second: '*',
        min: '*',
        hour: '*',
        day: '*',
        mouth: '*',
        week: '?',
        year: ''
      }
      for (var j in this.contabValueObj) {
        this.changeRadio(j, this.contabValueObj[j])
      }
    }
  },
  computed: {
    contabValueString: function () {
      var obj = this.contabValueObj
      var str =
        obj.second +
        ' ' +
        obj.min +
        ' ' +
        obj.hour +
        ' ' +
        obj.day +
        ' ' +
        obj.mouth +
        ' ' +
        obj.week +
        (obj.year === '' ? '' : ' ' + obj.year)
      return str
    }
  },
  components: {
    CrontabSecond,
    CrontabMin,
    CrontabHour,
    CrontabDay,
    CrontabMouth,
    CrontabWeek,
    CrontabYear,
    CrontabResult
  },
  watch: {
    expression: 'resolveExp',
    hideComponent (value) {
      // 隐藏部分组件
    }
  },
  mounted: function () {
    this.resolveExp()
  }
}
</script>
<style scoped>
.pop_btn {
  text-align: center;
  margin-top: 20px;
}
.popup-main {
  position: relative;
  margin: 10px auto;
  background: #fff;
  border-radius: 5px;
  font-size: 12px;
  overflow: hidden;
}
.popup-title {
  overflow: hidden;
  line-height: 34px;
  padding-top: 6px;
  background: #f2f2f2;
}
.popup-result {
  box-sizing: border-box;
  line-height: 24px;
  margin: 25px auto;
  padding: 15px 10px 10px;
  border: 1px solid #ccc;
  position: relative;
}
.popup-result .title {
  position: absolute;
  top: -28px;
  left: 50%;
  width: 140px;
  font-size: 14px;
  margin-left: -70px;
  text-align: center;
  line-height: 30px;
  background: #fff;
}
.popup-result table {
  text-align: center;
  width: 100%;
  margin: 0 auto;
}
.popup-result table span {
  display: block;
  width: 100%;
  font-family: arial;
  line-height: 30px;
  height: 30px;
  white-space: nowrap;
  overflow: hidden;
  border: 1px solid #e8e8e8;
}
.popup-result-scroll {
  font-size: 12px;
  line-height: 24px;
  height: 10em;
  overflow-y: auto;
}
</style>
