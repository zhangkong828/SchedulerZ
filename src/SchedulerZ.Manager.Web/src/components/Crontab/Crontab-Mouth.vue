<template>
  <a-form-model>
    <a-radio-group v-model="radioValue">
      <a-form-model-item>
        <a-radio :value="1">
          月，允许的通配符[, - * /]
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="2">
          周期从
          <a-input-number v-model.number="cycle01" :min="1" :max="12" /> -
          <a-input-number v-model.number="cycle02" :min="1" :max="12" /> 月
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="3">
          从
          <a-input-number v-model.number="average01" :min="1" :max="12" /> 月开始，每
          <a-input-number v-model.number="average02" :min="1" :max="12" /> 月月执行一次
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="4">
          指定
          <a-select :allowClear="true" v-model="checkboxList" placeholder="可多选" mode="multiple" style="width:100%">
            <a-select-option v-for="item in 12" :key="item">{{ item }}</a-select-option>
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
			cycle01: 1,
			cycle02: 2,
			average01: 1,
			average02: 1,
			checkboxList: [],
			checkNum: this.check
		}
	},
	name: 'CrontabMouth',
	// eslint-disable-next-line vue/require-prop-types
	props: ['check', 'cron'],
	methods: {
		// 单选按钮值变化时
		radioChange () {
			if (this.radioValue === 1) {
				this.$emit('update', 'mouth', '*')
				this.$emit('update', 'year', '*')
			} else {
				if (this.cron.day === '*') {
					this.$emit('update', 'day', '0', 'mouth')
				}
				if (this.cron.hour === '*') {
					this.$emit('update', 'hour', '0', 'mouth')
				}
				if (this.cron.min === '*') {
					this.$emit('update', 'min', '0', 'mouth')
				}
				if (this.cron.second === '*') {
					this.$emit('update', 'second', '0', 'mouth')
				}
			}
			switch (this.radioValue) {
				case 2:
					this.$emit('update', 'mouth', this.cycle01 + '-' + this.cycle02)
					break
				case 3:
					this.$emit('update', 'mouth', this.average01 + '/' + this.average02)
					break
				case 4:
					this.$emit('update', 'mouth', this.checkboxString)
					break
			}
		},
		// 周期两个值变化时
		cycleChange () {
			if (this.radioValue === 2) {
				this.$emit('update', 'mouth', this.cycleTotal)
			}
		},
		// 平均两个值变化时
		averageChange () {
			if (this.radioValue === 3) {
				this.$emit('update', 'mouth', this.averageTotal)
			}
		},
		// checkbox值变化时
		checkboxChange () {
			if (this.radioValue === 4) {
				this.$emit('update', 'mouth', this.checkboxString)
			}
		}
	},
	watch: {
		'radioValue': 'radioChange',
		'cycleTotal': 'cycleChange',
		'averageTotal': 'averageChange',
		'checkboxString': 'checkboxChange'
	},
	computed: {
		// 计算两个周期值
		cycleTotal: function () {
			return this.checkNum(this.cycle01, 1, 12) + '-' + this.checkNum(this.cycle02, 1, 12)
		},
		// 计算平均用到的值
		averageTotal: function () {
			return this.checkNum(this.average01, 1, 12) + '/' + this.checkNum(this.average02, 1, 12)
		},
		// 计算勾选的checkbox值合集
		checkboxString: function () {
			var str = this.checkboxList.join()
			return str === '' ? '*' : str
		}
	}
}
</script>
