<template>
  <a-form-model>
    <a-radio-group v-model="radioValue">
      <a-form-model-item>
        <a-radio :value="1">
          日，允许的通配符[, - * / L M]
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="2">
          不指定
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="3">
          周期从
          <a-input-number v-model.number="cycle01" :min="0" :max="31" /> -
          <a-input-number v-model.number="cycle02" :min="0" :max="31" /> 日
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="4">
          从
          <a-input-number v-model.number="average01" :min="0" :max="31" /> 号开始，每
          <a-input-number v-model.number="average02" :min="0" :max="31" /> 日执行一次
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="5">
          每月
          <a-input-number v-model.number="workday" :min="0" :max="31" /> 号最近的那个工作日
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="6">
          本月最后一天
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="7">
          指定
          <a-select :allowClear="true" v-model="checkboxList" placeholder="可多选" mode="multiple" style="width:100%">
            <a-select-option v-for="item in 31" :key="item">{{ item }}</a-select-option>
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
			radioValue: 1,
			workday: 1,
			cycle01: 1,
			cycle02: 2,
			average01: 1,
			average02: 1,
			checkboxList: [],
			checkNum: this.$options.propsData.check
		}
	},
	name: 'CrontabDay',
	// eslint-disable-next-line vue/require-prop-types
	props: ['check', 'cron'],
	methods: {
		// 单选按钮值变化时
		radioChange () {
			('day rachange')
			if (this.radioValue === 1) {
				this.$emit('update', 'day', '*', 'day')
				this.$emit('update', 'week', '?', 'day')
				this.$emit('update', 'mouth', '*', 'day')
			} else {
				if (this.cron.hour === '*') {
					this.$emit('update', 'hour', '0', 'day')
				}
				if (this.cron.min === '*') {
					this.$emit('update', 'min', '0', 'day')
				}
				if (this.cron.second === '*') {
					this.$emit('update', 'second', '0', 'day')
				}
			}

			switch (this.radioValue) {
				case 2:
					this.$emit('update', 'day', '?')
					break
				case 3:
					this.$emit('update', 'day', this.cycle01 + '-' + this.cycle02)
					break
				case 4:
					this.$emit('update', 'day', this.average01 + '/' + this.average02)
					break
				case 5:
					this.$emit('update', 'day', this.workday + 'W')
					break
				case 6:
					this.$emit('update', 'day', 'L')
					break
				case 7:
					this.$emit('update', 'day', this.checkboxString)
					break
			}
		},
		// 周期两个值变化时
		cycleChange () {
			if (this.radioValue === 3) {
				this.$emit('update', 'day', this.cycleTotal)
			}
		},
		// 平均两个值变化时
		averageChange () {
			if (this.radioValue === 4) {
				this.$emit('update', 'day', this.averageTotal)
			}
		},
		// 最近工作日值变化时
		workdayChange () {
			if (this.radioValue === 5) {
				this.$emit('update', 'day', this.workday + 'W')
			}
		},
		// checkbox值变化时
		checkboxChange () {
			if (this.radioValue === 7) {
				this.$emit('update', 'day', this.checkboxString)
			}
		},
		// 父组件传递的week发生变化触发
		weekChange () {
			// 判断week值与day不能同时为“?”
			if (this.cron.week === '?' && this.radioValue === '2') {
				this.radioValue = '1'
			} else if (this.cron.week !== '?' && this.radioValue !== '2') {
				this.radioValue = '2'
			}
		}
	},
	watch: {
		'radioValue': 'radioChange',
		'cycleTotal': 'cycleChange',
		'averageTotal': 'averageChange',
		'workdayCheck': 'workdayChange',
		'checkboxString': 'checkboxChange'
	},
	computed: {
		// 计算两个周期值
		cycleTotal: function () {
			return this.checkNum(this.cycle01, 1, 31) + '-' + this.checkNum(this.cycle02, 1, 31)
		},
		// 计算平均用到的值
		averageTotal: function () {
			return this.checkNum(this.average01, 1, 31) + '/' + this.checkNum(this.average02, 1, 31)
		},
		// 计算工作日格式
		workdayCheck: function () {
			return this.checkNum(this.workday, 1, 31)
		},
		// 计算勾选的checkbox值合集
		checkboxString: function () {
			var str = this.checkboxList.join()
			return str === '' ? '*' : str
		}
	}
}
</script>
