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

  </a-card>
</template>

<script>
import { STable } from '@/components'
import { getServiceList } from '@/api/route'

const columns = [
  {
    title: 'Id',
    dataIndex: 'id'
  },
  {
    title: '名称',
    dataIndex: 'name'
  },
   {
    title: '地址',
    dataIndex: 'address'
  },
  {
    title: '端口',
    dataIndex: 'port'
  },
  {
    title: 'Tag',
    dataIndex: 'tag'
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
      roleList: [],
      // 查询参数
      queryParam: {},
      // 表头
      columns,
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return getServiceList(requestParameters)
          .then(res => {
            console.log('getServiceList', res)
            return res.data
          })
      }
    }
  },
  filters: {
  },
  created () {
  },
  methods: {
    handleEdit (record) {
    },
    handleDelete (record) {
    },
    handleOk (e) {
      this.$refs.ruleForm.validate(valid => {
        if (valid) {
        }
      })
    },
    handleCancel () {
      this.visible = false
    }
  },
  watch: {
  }
}
</script>

<style lang="less" scoped>
</style>
