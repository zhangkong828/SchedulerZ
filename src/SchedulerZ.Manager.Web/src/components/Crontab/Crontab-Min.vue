<template>
  <a-form-model>
    <a-radio-group v-model="radioValue">
      <a-form-model-item>
        <a-radio :value="1">
          分钟，允许的通配符[, - * /]
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="2">
          周期从
          <a-input-number v-model.number="cycle01" :min="0" :max="59" /> -
          <a-input-number v-model.number="cycle02" :min="0" :max="59" /> 分钟
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="3">
          从
          <a-input-number v-model.number="average01" :min="0" :max="59" /> 分钟开始，每
          <a-input-number v-model.number="average02" :min="1" :max="59" /> 分钟执行一次
        </a-radio>
      </a-form-model-item>

      <a-form-model-item>
        <a-radio :value="4">
          指定
          <a-select :allowClear="true" v-model="checkboxList" placeholder="可多选" mode="multiple" style="width:100%">
            <a-select-option v-for="item in 60" :key="item-1">{{ item-1 }}</a-select-option>
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
			average01: 0,
			average02: 1,
			checkboxList: [],
			checkNum: this.$options.propsData.check
		}
	},
	name: 'CrontabMin',
	// eslint-disable-next-line vue/require-prop-types
	props: ['check', 'cron'],
	methods: {
		// 单选按钮值变化时
		radioChange () {
			if (this.radioValue !== 1 && this.cron.second === '*') {
				this.$emit('update', 'second', '0', 'min')
			}
			switch (this.radioValue) {
				case 1:
					this.$emit('update', 'min', '*', 'min')
					this.$emit('update', 'hour', '*', 'min')
					break
				case 2:
					this.$emit('update', 'min', this.cycle01 + '-' + this.cycle02, 'min')
					break
				case 3:
					this.$emit('update', 'min', this.average01 + '/' + this.average02, 'min')
					break
				case 4:
					this.$emit('update', 'min', this.checkboxString, 'min')
					break
			}
		},
		// 周期两个值变化时
		cycleChange () {
			if (this.radioValue === 2) {
				this.$emit('update', 'min', this.cycleTotal, 'min')
			}
		},
		// 平均两个值变化时
		averageChange () {
			if (this.radioValue === 3) {
				this.$emit('update', 'min', this.averageTotal, 'min')
			}
		},
		// checkbox值变化时
		checkboxChange () {
			if (this.radioValue === 4) {
				this.$emit('update', 'min', this.checkboxString, 'min')
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
			return this.checkNum(this.cycle01, 0, 59) + '-' + this.checkNum(this.cycle02, 0, 59)
		},
		// 计算平均用到的值
		averageTotal: function () {
			return this.checkNum(this.average01, 0, 59) + '/' + this.checkNum(this.average02, 1, 59)
		},
		// 计算勾选的checkbox值合集
		checkboxString: function () {
			var str = this.checkboxList.join()
			return str === '' ? '*' : str
		}
	}
}
</script>
