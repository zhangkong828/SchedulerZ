<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form-model layout="inline">
        <a-row :gutter="48">
          <a-col :md="8" :sm="24">
            <a-form-model-item label="名称">
              <a-input placeholder="请输入" v-model="queryParam.name"/>
            </a-form-model-item>
          </a-col>
          <a-col :md="8" :sm="24">
            <a-form-model-item label="状态">
              <a-select v-model.number="queryParam.status" placeholder="请选择" default-value="0">
                <a-select-option :value="0">全部</a-select-option>
                <a-select-option :value="1">正常</a-select-option>
                <a-select-option :value="2">禁用</a-select-option>
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

    <s-table
      ref="table"
      row-key="id"
      :columns="columns"
      :data="loadData"
      showPagination="auto"
    >
      <a-avatar slot="avatar" size="large" shape="square" slot-scope="text" :src="text"/>
      <a-tag color="blue" slot="status" slot-scope="text">{{ text | statusFilter }}</a-tag>
      <span slot="action" slot-scope="text, record">
        <a @click="handleEdit(record)">编辑</a>
        <a-divider type="vertical" />
        <a @click="handleDelete(record)">删除</a>
      </span>
    </s-table>

    <a-modal
      title="操作"
      style="top: 20px;"
      :width="800"
      v-model="visible"
      @ok="handleOk"
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
          label="用户名"
          prop="username"
        >
          <a-input placeholder="用户名" v-model="form.username"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="状态"
          prop="status"
        >
          <a-select v-model="form.status">
            <a-select-option :value="1">正常</a-select-option>
            <a-select-option :value="2">禁用</a-select-option>
          </a-select>
        </a-form-model-item>

      </a-form-model>
    </a-modal>

  </a-card>
</template>

<script>
import { STable } from '@/components'
import { getUserList, modifyUser, deleteUser } from '@/api/system'

const STATUS = {
  1: '正常',
  2: '禁用'
}

const columns = [
  {
    title: '#',
    dataIndex: 'id'
  },
  {
    title: '头像',
    dataIndex: 'avatar',
     scopedSlots: { customRender: 'avatar' }
  },
  {
    title: '名称',
    dataIndex: 'name'
  },
  {
    title: '状态',
    dataIndex: 'status',
    scopedSlots: { customRender: 'status' }
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    sorter: true
  },
   {
    title: '最后登录IP',
    dataIndex: 'lastLoginIp'
  },
   {
    title: '最后登录时间',
    dataIndex: 'lastLoginTime',
    sorter: true
  },
   {
    title: '操作',
    width: '150px',
    dataIndex: 'action',
    scopedSlots: { customRender: 'action' }
  }
]

export default {
  name: 'TableList',
  components: {
    STable
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
        username: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '必填项', trigger: 'blur' }
        ]
      },
      // 查询参数
      queryParam: {},
      // 表头
      columns,
      // 加载数据方法 必须为 Promise 对象
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return getUserList(requestParameters)
          .then(res => {
            console.log('getUserList', res)
            return res.data
          })
      }
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
    handleEdit (record) {
      this.form = Object.assign({}, record)
      this.visible = true
    },
    handleDelete (record) {
      deleteUser(record.id).then(res => {
        this.$refs.table.refresh()
      })
    },
    handleOk (e) {
      const _this = this
      this.$refs.ruleForm.validate(valid => {
        if (valid) {
          modifyUser(this.form)
            .then((res) => {
              _this.$refs.table.refresh()
            })
        }
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
