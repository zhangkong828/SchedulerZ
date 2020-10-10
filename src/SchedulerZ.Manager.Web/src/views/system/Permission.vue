<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="48">
          <a-col :md="8" :sm="24">
            <a-form-item label="权限">
              <a-input placeholder="请输入" v-model="queryParam.name"/>
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24">
            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="$refs.table.refresh(true)">查询</a-button>
              <a-button style="margin-left: 8px" @click="() => this.queryParam = {}">重置</a-button>
            </span>
          </a-col>
        </a-row>
      </a-form>
    </div>

    <div class="table-operator">
      <a-button type="primary" icon="plus" @click="handleAdd">新建</a-button>
    </div>

    <s-table ref="table" :columns="columns" :data="loadData" :defaultExpandAllRows="true" row-key="id">
      <span slot="icon" slot-scope="text">
        <a-icon :type="text" />
      </span>

      <span slot="actions" slot-scope="text, record">
        <a-tag v-for="(action, index) in record.actionList" :key="index">{{ action.describe }}</a-tag>
      </span>

      <span slot="action" slot-scope="text, record">
        <a @click="handleEdit(record)">编辑</a>
        <a-divider type="vertical" />
        <a @click="handleDelete(record)">删除</a>
      </span>
    </s-table>

    <a-modal title="操作" :width="800" v-model="visible" @ok="handleOk">
      <a-form :model="mdl">
        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="名称"
        >
          <a-input placeholder="名称" v-model="mdl.title" name="title" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="唯一识别码"
        >
          <a-input placeholder="唯一识别码" v-model="mdl.name" name="name" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="icon"
        >
          <a-input-search v-model="mdl.icon" name="icon" @search="handleIcon(mdl.icon)">
            <template v-slot:prefix><a-icon :type="mdl.icon" /></template>
            <template v-slot:enterButton>
              <a-button>选择</a-button>
            </template>
          </a-input-search>
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="路径"
        >
          <a-input placeholder="path" v-model="mdl.path" name="path" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="组件"
        >
          <a-input placeholder="" v-model="mdl.component" name="component" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="重定向"
        >
          <a-input placeholder="" v-model="mdl.redirect" name="redirect" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="跳转目标"
        >
          <a-input placeholder="_blank|_self|_top|_parent" v-model="mdl.target" name="target" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="排序"
        >
          <a-input placeholder="" v-model="mdl.sort" name="sort" />
        </a-form-item>

        <a-form-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="是否显示"
        >
          <a-switch v-model="mdl.show" />
        </a-form-item>

        <a-divider />

        <a-form-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="赋予权限">
          <a-select style="width: 100%" mode="multiple" v-model="mdl.permission" :allowClear="true">
            <a-select-option v-for="(action, index) in permissionList" :key="index" :value="action.value">{{
              action.label
            }}</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>

    <a-modal title="选择Icon" :width="800" v-model="visibleIcon" @ok="handleIconOk">
      <a-card :bordered="false">
        <icon-selector v-model="currentSelectedIcon" @change="handleIconChange" />
      </a-card>
    </a-modal>
  </a-card>
</template>

<script>
import { STable, IconSelector } from '@/components'
import { getPermissions } from '@/api/system'

export default {
  name: 'TableList',
  components: {
    STable,
    IconSelector
  },
  data () {
    return {
      visible: false,
      visibleIcon: false,
      currentSelectedIcon: 'pause-circle',
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 }
      },
      form: null,
      mdl: {},

      // 查询参数
      queryParam: {},
      // 表头
      columns: [
        {
          title: '名称',
          dataIndex: 'title'
        },
        {
          title: '唯一识别码',
          dataIndex: 'name'
        },
        {
          title: '菜单icon',
          dataIndex: 'icon',
          scopedSlots: { customRender: 'icon' }
        },
        {
          title: '可操作权限',
          dataIndex: 'actions',
          scopedSlots: { customRender: 'actions' }
        },
        {
          title: '排序',
          dataIndex: 'sort'
        },
        {
          title: '操作',
          width: '150px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' }
        }
      ],
      // 向后端拉取可以用的操作列表
      permissionList: null,
      // 加载数据方法 必须为 Promise 对象
      loadData: (parameter) => {
        // return this.$http.get('/permission', {
        //   params: Object.assign(parameter, this.queryParam)
        // }).then(res => {
        //   const result = res.result
        //   result.data.map(permission => {
        //     permission.actionList = JSON.parse(permission.actionData)
        //     return permission
        //   })
        //   return result
        // })
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return getPermissions(requestParameters).then((res) => {
          console.log('getPermissions', res)
          return res.data
        })
      },

      selectedRowKeys: [],
      selectedRows: []
    }
  },
  created () {
    this.loadPermissionList()
  },
  methods: {
    loadPermissionList () {
      // permissionList
      new Promise((resolve) => {
        const data = [
          { label: '新增', value: 'add', defaultChecked: false },
          { label: '查询', value: 'get', defaultChecked: false },
          { label: '修改', value: 'update', defaultChecked: false },
          { label: '列表', value: 'query', defaultChecked: false },
          { label: '删除', value: 'delete', defaultChecked: false },
          { label: '导入', value: 'import', defaultChecked: false },
          { label: '导出', value: 'export', defaultChecked: false }
        ]
        setTimeout(resolve(data), 1500)
      }).then((res) => {
        this.permissionList = res
      })
    },
    handleIcon (icon) {
      this.currentSelectedIcon = icon
      this.visibleIcon = true
    },
    handleIconChange (icon) {
      console.log('change Icon', icon)
      this.currentSelectedIcon = icon
    },
    handleIconOk () {
      this.mdl.icon = this.currentSelectedIcon
      this.visibleIcon = false
    },
    handleAdd () {
      this.mdl = {}
      this.visible = true
    },
    handleEdit (record) {
      this.mdl = Object.assign({}, record)
      this.visible = true
    },
    handleDelete (record) {},
    handleOk () {}
  }
}
</script>
