<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="48">
          <a-col :md="8" :sm="24">
            <a-form-model-item label="权限">
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
      </a-form>
    </div>

    <div class="table-operator">
      <a-button type="primary" icon="plus" @click="handleAdd">新建</a-button>
    </div>

    <s-table ref="table" :columns="columns" :data="loadData" :defaultExpandAllRows="true" row-key="id">
      <span slot="icon" slot-scope="text">
        <a-icon v-if="text" :type="text" />
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

    <a-modal title="操作" :width="800" v-model="visible" :footer="null">
      <a-form-model ref="ruleForm" :model="form" :rules="rules">
        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="名称"
          prop="title"
        >
          <a-input placeholder="名称" v-model="form.title"/>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="唯一识别码"
          prop="name"
        >
          <a-input placeholder="唯一识别码" v-model="form.name" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="icon"
          prop="icon"
        >
          <a-input-search v-model="form.icon" name="icon" @search="handleIcon(form.icon)">
            <template v-slot:prefix><a-icon v-if="form.icon" :type="form.icon" /></template>
            <template v-slot:enterButton>
              <a-button>选择</a-button>
            </template>
          </a-input-search>
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="路径"
          prop="path"
        >
          <a-input placeholder="path" v-model="form.path" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="组件"
          prop="component"
        >
          <a-input placeholder="" v-model="form.component" name="component" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="重定向"
          prop="redirect"
        >
          <a-input placeholder="" v-model="form.redirect" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="跳转目标"
          prop="target"
        >
          <a-input placeholder="_blank|_self|_top|_parent" v-model="form.target" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="排序"
          prop="sort"
        >
          <a-input placeholder="" v-model="form.sort" />
        </a-form-model-item>

        <a-form-model-item
          :labelCol="labelCol"
          :wrapperCol="wrapperCol"
          label="是否显示"
          prop="show"
        >
          <a-switch v-model="form.show" />
        </a-form-model-item>

        <a-divider />

        <a-form-model-item :labelCol="labelCol" :wrapperCol="wrapperCol" label="赋予权限">
          <a-select style="width: 100%" mode="multiple" v-model="form.permission" :allowClear="true">
            <a-select-option v-for="(action, index) in permissionList" :key="index" :value="action.value">{{
              action.label
            }}</a-select-option>
          </a-select>
        </a-form-model-item>
        <a-form-model-item :wrapper-col="{ span: 14, offset: 4 }">
          <a-button type="primary" @click="onSubmit">
            确定
          </a-button>
          <a-button style="margin-left: 10px;" @click="resetForm">
            重置
          </a-button>
        </a-form-model-item>
      </a-form-model>
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
import { getPermissions, modifyPermission } from '@/api/system'

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
      form: {},
      mdl: {
        id: 0,
        title: '',
        path: '',
        name: '',
        component: '',
        permission: '',
        icon: '',
        hiddenHeaderContent: false,
        target: '',
        show: true,
        hideChildren: false,
        redirect: '',
        remark: '',
        parentId: 0,
        sort: 0
      },

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
      selectedRows: [],
      rules: {
        title: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        path: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        component: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        sort: [
          { required: true, message: '必填项', trigger: 'blur' }
        ]
      }
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
      this.form.icon = this.currentSelectedIcon
      this.visibleIcon = false
    },
    handleAdd () {
      this.form = Object.assign({}, this.mdl)
      this.visible = true
    },
    handleEdit (record) {
      this.form = Object.assign({}, record)
      this.visible = true
    },
    handleDelete (record) {},
    onSubmit () {
       this.$refs.ruleForm.validate(valid => {
        if (valid) {
          modifyPermission(this.form)
            .then((res) => {
              this.$refs.table.refresh()
              this.visible = false
            })
        }
      })
    },
    resetForm () {
      this.$refs.ruleForm.resetFields()
    }
  }
}
</script>
