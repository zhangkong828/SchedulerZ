<template>
  <a-modal
    title="操作"
    :width="800"
    :visible="visible"
    :confirmLoading="confirmLoading"
    @ok="handleOk"
    @cancel="handleCancel"
  >
    <a-form-model ref="ruleForm" :model="form" :rules="rules">
      <a-form-model-item
        :labelCol="labelCol"
        :wrapperCol="wrapperCol"
        label="识别码"
        prop="identify"
      >
        <a-input placeholder="识别码" v-model="form.identify"/>
      </a-form-model-item>
      <a-form-model-item
        :labelCol="labelCol"
        :wrapperCol="wrapperCol"
        label="角色名称"
        prop="name"
      >
        <a-input placeholder="角色名称" v-model="form.name"/>
      </a-form-model-item>
      <a-form-model-item
        :labelCol="labelCol"
        :wrapperCol="wrapperCol"
        label="备注"
        prop="remark"
      >
        <a-input placeholder="备注" type="textarea" v-model="form.remark"/>
      </a-form-model-item>

      <a-card :bordered="false">
        <a-row :gutter="8">
          <a-col :span="5">
            <s-tree
              :dataSource="routerTree"
              @click="routerTreeHandleClick"></s-tree>
          </a-col>
          <a-col :span="19">
            <s-table
              ref="actionTable"
              size="default"
              :columns="columns"
              :alert="false"
              :rowSelection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }"
            >
              <span slot="action" slot-scope="text, record">
                <template v-if="$auth('table.update')">
                  <a @click="handleEdit(record)">编辑</a>
                  <a-divider type="vertical" />
                </template>
                <a-dropdown>
                  <a class="ant-dropdown-link">
                    更多 <a-icon type="down" />
                  </a>
                  <a-menu slot="overlay">
                    <a-menu-item>
                      <a href="javascript:;">详情</a>
                    </a-menu-item>
                    <a-menu-item v-if="$auth('table.disable')">
                      <a href="javascript:;">禁用</a>
                    </a-menu-item>
                    <a-menu-item v-if="$auth('table.delete')">
                      <a href="javascript:;">删除</a>
                    </a-menu-item>
                  </a-menu>
                </a-dropdown>
              </span>
            </s-table>
          </a-col>
        </a-row>
      </a-card>

    </a-form-model>
  </a-modal>
</template>

<script>
// import { getPermissions } from '@/api/manage'
// import { actionToObject } from '@/utils/permissions'
// import pick from 'lodash.pick'
import STree from '@/components/Tree/Tree'
import { STable } from '@/components'
export default {
  name: 'RoleModal',
  components: {
    STree,
    STable
  },
  data () {
    return {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 5 }
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 }
      },
      visible: false,
      confirmLoading: false,
      mdl: {},
      form: {},
      permissions: [],
      rules: {
        parentId: [
          { required: true, message: '必填项', trigger: 'change' }
        ],
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
      },
      routerTree: [],
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
          dataIndex: 'permission',
          scopedSlots: { customRender: 'permission' }
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
      selectedRowKeys: [],
      selectedRows: []
    }
  },
  created () {
    // this.loadPermissions()
  },
  methods: {
    add () {
      this.edit({ id: 0 })
    },
    edit (record) {
      this.mdl = Object.assign({}, record)
      this.visible = true

      // // 有权限表，处理勾选
      // if (this.mdl.permissions && this.permissions) {
      //   // 先处理要勾选的权限结构
      //   const permissionsAction = {}
      //   this.mdl.permissions.forEach(permission => {
      //     permissionsAction[permission.permissionId] = permission.actionEntitySet.map(entity => entity.action)
      //   })
      //   // 把权限表遍历一遍，设定要勾选的权限 action
      //   this.permissions.forEach(permission => {
      //     permission.selected = permissionsAction[permission.id] || []
      //   })
      // }

      // this.$nextTick(() => {
      //   this.form.setFieldsValue(pick(this.mdl, 'id', 'name', 'status', 'describe'))
      // })
      console.log('this.mdl', this.mdl)
    },
    close () {
      this.$emit('close')
      this.visible = false
    },
    handleOk () {
      const _this = this
      // 触发表单验证
      this.form.validateFields((err, values) => {
        // 验证表单没错误
        if (!err) {
          console.log('form values', values)

          _this.confirmLoading = true
          // 模拟后端请求 2000 毫秒延迟
          new Promise((resolve) => {
            setTimeout(() => resolve(), 2000)
          }).then(() => {
            // Do something
            _this.$message.success('保存成功')
            _this.$emit('ok')
          }).catch(() => {
            // Do something
          }).finally(() => {
            _this.confirmLoading = false
            _this.close()
          })
        }
      })
    },
    handleCancel () {
      this.close()
    },
    routerTreeHandleClick (e) {
      this.queryParam = {
        key: e.key
      }
      this.$refs.actionTable.refresh(true)
    },
    onSelectChange (selectedRowKeys, selectedRows) {
      this.selectedRowKeys = selectedRowKeys
      this.selectedRows = selectedRows
    },
    onChangeCheck (permission) {
      permission.indeterminate = !!permission.selected.length && (permission.selected.length < permission.actionsOptions.length)
      permission.checkedAll = permission.selected.length === permission.actionsOptions.length
    },
    onChangeCheckAll (e, permission) {
      Object.assign(permission, {
        selected: e.target.checked ? permission.actionsOptions.map(obj => obj.value) : [],
        indeterminate: false,
        checkedAll: e.target.checked
      })
    }
    // loadPermissions () {
    //   const that = this
    //   getPermissions().then(res => {
    //     const result = res.result
    //     that.permissions = result.map(permission => {
    //       const options = actionToObject(permission.actionData)
    //       permission.checkedAll = false
    //       permission.selected = []
    //       permission.indeterminate = false
    //       permission.actionsOptions = options.map(option => {
    //         return {
    //           label: option.describe,
    //           value: option.action
    //         }
    //       })
    //       return permission
    //     })
    //   })
    // }

  }
}
</script>

<style scoped>

</style>
