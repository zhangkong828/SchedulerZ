<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form-model layout="inline">
        <a-row :gutter="48">
          <a-col :md="8" :sm="24">
            <a-form-model-item label="Id/名称">
              <a-input placeholder="请输入" v-model="queryParam.name"/>
            </a-form-model-item>
          </a-col>
          <a-col :md="8" :sm="24">
            <a-form-model-item label="状态">
              <a-select v-model.number="queryParam.status" placeholder="请选择" default-value="-2">
                <a-select-option :value="-2">全部</a-select-option>
                <a-select-option :value="0">已停止</a-select-option>
                <a-select-option :value="1">运行中</a-select-option>
                <a-select-option :value="2">已暂停</a-select-option>
              </a-select>
            </a-form-model-item>
          </a-col>
          <a-col :md="8" :sm="24">
            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
              <a-button style="margin-left: 8px" @click="() => this.queryParam = {}">重置</a-button>
            </span>
          </a-col>
        </a-row>
      </a-form-model>
    </div>
    <div class="table-operator">
      <a-button type="primary" icon="plus" @click="handleAdd">新建</a-button>
    </div>
    <s-table
      ref="table"
      row-key="id"
      :columns="columns"
      :data="loadData"
      showPagination="auto"
      :expandRowByClick="true"
    >
      <a-tag :color="text==0?'red':(text==1?'green':'purple')" slot="status" slot-scope="text">{{ text | statusFilter }}</a-tag>
      <span slot="nodeHost" slot-scope="text, record"><p v-if="record.nodeHost">{{ record.nodeHost+':'+record.nodePort }}</p></span>
      <div slot="expandedRowRender" slot-scope="record" style="margin: 0">
        <a-descriptions title="详细" :column="2">
          <a-descriptions-item label="程序集名称">{{ record.assemblyName }}</a-descriptions-item>
          <a-descriptions-item label="类名">{{ record.className }}</a-descriptions-item>
          <a-descriptions-item label="文件名">{{ record.filePath }}</a-descriptions-item>
          <a-descriptions-item label="备注">{{ record.remark }}</a-descriptions-item>
          <a-descriptions-item label="自定义参数">{{ record.customParamsJson }}</a-descriptions-item>
          <a-descriptions-item label=""></a-descriptions-item>
          <a-descriptions-item label="简易任务">{{ record.isSimple?'是':'否' }}</a-descriptions-item>
          <a-descriptions-item label=""></a-descriptions-item>
          <a-descriptions-item label="cron表达式" v-if="!record.isSimple">{{ record.cronExpression }}</a-descriptions-item>
          <a-descriptions-item label="" v-if="!record.isSimple"></a-descriptions-item>
          <a-descriptions-item label="间隔秒数" v-if="record.isSimple">{{ record.intervalSeconds }}</a-descriptions-item>
          <a-descriptions-item label="重复次数" v-if="record.isSimple">{{ record.repeatCount }}</a-descriptions-item>
        </a-descriptions>
      </div>
      <span slot="action" slot-scope="text, record">
        <a @click="handleEdit(record)" :disabled="record.status!==0">编辑</a>
        <a-divider type="vertical" />
        <a-popconfirm
          title="确定要删除?"
          ok-text="Yes"
          cancel-text="No"
          :disabled="record.status!==0"
          @confirm="handleDelete(record)"
        >
          <a href="#" :disabled="record.status!==0">删除</a>
        </a-popconfirm>
        <a-divider type="vertical" />
        <a-dropdown>
          <a class="ant-dropdown-link">
            操作任务 <a-icon type="down" />
          </a>
          <a-menu slot="overlay">
            <a-menu-item>
              <a @click="handleStartJob(record)" :disabled="record.status!==0">启动任务</a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handlePauseJob(record)" :disabled="record.status!==1">暂停任务</a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handleResumeJob(record)" :disabled="record.status!==2">恢复任务</a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handleStopJob(record)" :disabled="record.status<=0">停止任务</a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handleRunOnceNowJob(record)" :disabled="record.status!==1">立即运行一次</a>
            </a-menu-item>
          </a-menu>
        </a-dropdown>
      </span>
    </s-table>

    <a-modal
      title="操作"
      style="top: 20px;"
      :width="800"
      :visible="visible"
      @ok="handleOk"
      @cancel="handleCancel"
    >
      <a-form-model ref="ruleForm" :model="form" :rules="rules">

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="名称"
          prop="name"
        >
          <a-input placeholder="名称" v-model="form.name"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="备注"
        >
          <a-input placeholder="备注" type="textarea" v-model="form.remark"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="程序集名称"
          prop="assemblyName"
        >
          <a-input placeholder="程序集名称" v-model="form.assemblyName"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="类名"
          prop="className"
        >
          <a-input placeholder="类名" v-model="form.className"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="任务文件"
          prop="filePath"
        >
          <a-upload
            name="file"
            :fileList="files"
            accept="application/zip"
            :customRequest="customUploadRequest"
            :remove="handleRemoveFile"
          >
            <a-button> <a-icon type="upload" /> 上传zip</a-button>
          </a-upload>
          <a-input type="hidden" v-model="form.filePath"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="自定义参数"
        >
          <a-input placeholder="自定义参数" type="textarea" v-model="form.customParamsJson"/>
        </a-form-model-item>

        <a-form-model-item
          v-if="!form.isSimple"
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="cron表达式"
          prop="cronExpression"
        >
          <a-input placeholder="cron表达式" v-model="form.cronExpression"/>
          <a-button type="primary" @click="showCrontabDialog">生成cron</a-button>
          <a-modal title="生成cron" v-model="showCron" width="620px" :footer="null" :closable="false">
            <crontab ref="crontab" @hide="showCron=false" @fill="crontabFill" :expression="cronExpression"></crontab>
          </a-modal>
        </a-form-model-item>

        <a-form-model-item
          v-if="form.isSimple"
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="间隔秒数"
          prop="intervalSeconds"
        >
          <a-input placeholder="间隔秒数" v-model.number="form.intervalSeconds"/>
        </a-form-model-item>

        <a-form-model-item
          v-if="form.isSimple"
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="重复次数"
          prop="repeatCount"
        >
          <a-input placeholder="重复次数" v-model.number="form.repeatCount"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="简易任务"
        >
          <a-switch v-model="form.isSimple" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="生效时间"
        >
          <a-date-picker show-time placeholder="生效时间" v-model="form.startTime"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="失效时间"
        >
          <a-date-picker show-time placeholder="失效时间" v-model="form.endTime"/>
        </a-form-model-item>

      </a-form-model>
    </a-modal>

  </a-card>
</template>

<script>
import { STable, Crontab } from '@/components'
import { getJobList, modifyJob, deleteJob, uploadPackage, startJob, runOnceNowJob, pauseJob, resumeJob, stopJob } from '@/api/job'

const STATUS = {
  0: '已停止',
  1: '运行中',
  2: '已暂停'
}

const columns = [
  {
    title: '#',
    dataIndex: 'id'
  },
  {
    title: '任务名称',
    dataIndex: 'name'
  },
   {
    title: '上次运行时间',
    dataIndex: 'lastRunTime'
  },
   {
    title: '下次运行时间',
    dataIndex: 'nextRunTime'
  },
  {
    title: '总运行成功次数',
    dataIndex: 'totalRunCount'
  },
  {
    title: '任务状态',
    dataIndex: 'status',
    scopedSlots: { customRender: 'status' }
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    sorter: true
  },
  {
    title: '所属节点',
    dataIndex: 'nodeHost',
    scopedSlots: { customRender: 'nodeHost' }
  },
   {
    title: '操作',
    width: '200px',
    dataIndex: 'action',
    scopedSlots: { customRender: 'action' }
  }
]

export default {
  name: 'TableList',
  components: {
    STable,
    Crontab
  },
  data () {
    return {
      visible: false,
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 }
      },
      mdl: {},
      form: {},
      rules: {
        name: [
          { required: true, message: '必填项', trigger: 'blur', whitespace: true }
        ],
        assemblyName: [
          { required: true, message: '必填项', trigger: 'blur', whitespace: true }
        ],
        className: [
          { required: true, message: '必填项', trigger: 'blur', whitespace: true }
        ],
        cronExpression: [
          { required: true, message: '必填项', trigger: 'blur', whitespace: true }
        ],
        intervalSeconds: [
          { required: true, message: '请填写有效值', trigger: 'blur', type: 'integer' }
        ],
        repeatCount: [
          { required: true, message: '请填写有效值', trigger: 'blur', type: 'integer' }
        ],
        filePath: [
          { required: true, message: '请上传文件', trigger: 'change', whitespace: true }
        ]
      },
      roleList: [],
      // 查询参数
      queryParam: {},
      // 表头
      columns,
      // 加载数据方法 必须为 Promise 对象
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return getJobList(requestParameters)
          .then(res => {
            console.log('getJobList', res)
            return res.data
          })
      },
      files: [],
      cronExpression: '',
      showCron: false
    }
  },
  filters: {
    statusFilter (key) {
      return STATUS[key]
    }
  },
  created () {
  },
  methods: {
    customUploadRequest (data) {
      console.log(data)
      const formData = new FormData()
      formData.append('file', data.file)
      uploadPackage(formData).then((res) => {
        if (res.code === 1 && res.data && res.data.length > 0) {
          var file = res.data[0]
          if (file.success) {
            this.files.push({ uid: data.file.uid, name: file.fileName, status: 'done' })
            this.form.filePath = file.fileName
          }
        } else {
          this.$message.error(res.message)
        }
      })
    },
    handleRemoveFile (file) {
      this.files = []
      this.form.filePath = ''
    },
    showCrontabDialog () {
      this.cronExpression = this.form.cronExpression
      this.showCron = true
    },
    crontabFill (value) {
     this.form.cronExpression = value
    },
    handleAdd () {
      this.handleEdit({ id: '', show: false })
    },
    handleEdit (record) {
      this.form = Object.assign({}, record)
      this.files = []
      if (this.form.filePath) {
        this.files.push({ uid: (new Date()).valueOf(), name: this.form.filePath, status: 'done' })
      }
      this.visible = true
    },
    handleDelete (record) {
      deleteJob(record.id).then(res => {
        this.$refs.table.refresh()
      })
    },
    handleOk (e) {
      this.$refs.ruleForm.validate(valid => {
        if (valid) {
          console.log(this.form)
          modifyJob(this.form)
            .then((res) => {
              this.$refs.table.refresh()
              this.visible = false
            })
        }
      })
    },
    handleCancel () {
      this.visible = false
      this.$refs.ruleForm.resetFields()
    },
    handleStartJob (record) {
      startJob(record.id).then(res => {
        if (res.data.success) {
          this.$refs.table.refresh()
        }
        this.handleJobResult(res.data.success, res.data.message)
      })
    },
    handlePauseJob (record) {
      pauseJob(record.id).then(res => {
        this.$refs.table.refresh()
      })
     },
    handleResumeJob (record) {
      resumeJob(record.id).then(res => {
        this.$refs.table.refresh()
      })
    },
    handleStopJob (record) {
      stopJob(record.id).then(res => {
        this.$refs.table.refresh()
      })
     },
    handleRunOnceNowJob (record) {
      runOnceNowJob(record.id).then(res => {
        this.$refs.table.refresh()
      })
    },
    handleJobResult (flag, message) {
      var type = flag ? 'success' : 'error'
      this.$notification[type]({
        message: '操作结果',
        description: message
      })
    }
  },
  watch: {
  }
}
</script>

<style lang="less" scoped>
.permission-form {
  /deep/ .permission-group {
    margin-top: 0;
    margin-bottom: 0;
  }
}

</style>
