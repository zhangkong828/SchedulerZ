<template>
  <a-form-model>
    <a-radio-group v-model="radioValue">
      <a-form-model-item>
        <a-radio :value="1">
          周，允许的通配符[, - * / L #]
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="2">
          不指定
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="3">
          周期从星期
          <a-input-number v-model.number="cycle01" :min="1" :max="7" /> -
          <a-input-number v-model.number="cycle02" :min="1" :max="7" />
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="4">
          第
          <a-input-number v-model.number="average01" :min="1" :max="4" /> 周的星期
          <a-input-number v-model.number="average02" :min="1" :max="7" />
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="5">
          本月最后一个星期
          <a-input-number v-model="weekday" :min="1" :max="7" />
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="6">
          指定
          <a-select :allowClear="true" v-model="checkboxList" placeholder="可多选" mode="multiple" style="width:100%">
            <a-select-option v-for="(item,index) of weekList" :key="index+1">{{ item }}</a-select-option>
          </a-select>
        </a-radio>
      </a-form-model-item>
    </a-radio-group>
  </a-form-model>
</template>

<script>
export default {
	data () {
		return {
			radioValue: 2,
			weekday: 1,
			cycle01: 1,
			cycle02: 2,
			average01: 1,
			average02: 1,
			checkboxList: [],
			weekList: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'],
			checkNum: this.$options.propsData.check
		}
	},
	name: 'CrontabWeek',
	// eslint-disable-next-line vue/require-prop-types
	props: ['check', 'cron'],
	methods: {
		// 单选按钮值变化时
		radioChange () {
			if (this.radioValue === 1) {
				this.$emit('update', 'week', '*')
				this.$emit('update', 'year', '*')
			} else {
				if (this.cron.mouth === '*') {
					this.$emit('update', 'mouth', '0', 'week')
				}
				if (this.cron.day === '*') {
					this.$emit('update', 'day', '0', 'week')
				}
				if (this.cron.hour === '*') {
					this.$emit('update', 'hour', '0', 'week')
				}
				if (this.cron.min === '*') {
					this.$emit('update', 'min', '0', 'week')
				}
				if (this.cron.second === '*') {
					this.$emit('update', 'second', '0', 'week')
				}
			}
			switch (this.radioValue) {
				case 2:
					this.$emit('update', 'week', '?')
					break
				case 3:
					this.$emit('update', 'week', this.cycle01 + '-' + this.cycle02)
					break
				case 4:
					this.$emit('update', 'week', this.average01 + '#' + this.average02)
					break
				case 5:
					this.$emit('update', 'week', this.weekday + 'L')
					break
				case 6:
					this.$emit('update', 'week', this.checkboxString)
					break
			}
		},
		// 根据互斥事件，更改radio的值

		// 周期两个值变化时
		cycleChange () {
			if (this.radioValue === 3) {
				this.$emit('update', 'week', this.cycleTotal)
			}
		},
		// 平均两个值变化时
		averageChange () {
			if (this.radioValue === 4) {
				this.$emit('update', 'week', this.averageTotal)
			}
		},
		// 最近工作日值变化时
		weekdayChange () {
			if (this.radioValue === 5) {
				this.$emit('update', 'week', this.weekday + 'L')
			}
		},
		// checkbox值变化时
		checkboxChange () {
			if (this.radioValue === 6) {
				this.$emit('update', 'week', this.checkboxString)
			}
		}
	},
	watch: {
		'radioValue': 'radioChange',
		'cycleTotal': 'cycleChange',
		'averageTotal': 'averageChange',
		'weekdayCheck': 'weekdayChange',
		'checkboxString': 'checkboxChange'
	},
	computed: {
		// 计算两个周期值
		cycleTotal: function () {
			return this.checkNum(this.cycle01, 1, 7) + '-' + this.checkNum(this.cycle02, 1, 7)
		},
		// 计算平均用到的值
		averageTotal: function () {
			return this.checkNum(this.average01, 1, 4) + '#' + this.checkNum(this.average02, 1, 7)
		},
		// 最近的工作日（格式）
		weekdayCheck: function () {
			return this.checkNum(this.weekday, 1, 7)
		},
		// 计算勾选的checkbox值合集
		checkboxString: function () {
			var str = this.checkboxList.join()
			return str === '' ? '*' : str
		}
	}
}
</script>
