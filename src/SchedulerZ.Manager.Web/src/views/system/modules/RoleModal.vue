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
          <a-col :span="14">
            <a-tree
              v-model="routerTreeCheckedKeys"
              :tree-data="routerTree"
              :defaultSelectedKeys="routerTreeDefaultSelectedKeys"
              checkable
              checkStrictly
              defaultExpandAll
              @click="routerTreeHandleClick"
              @check="routerTreeHandleCheck"></a-tree>
          </a-col>
          <a-col :span="10">
            <a-table
              ref="actionTable"
              row-key="id"
              :columns="columns"
              :data-source="actionTableDatas"
              :rowSelection="{ selectedRowKeys: actionTableSelectedRowKeys, onChange: onActionTableSelectChange }"
            >
            </a-table>
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
import { getPermissionTree } from '@/api/system'
export default {
  name: 'RoleModal',
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
        identify: [
          { required: true, message: '必填项', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '必填项', trigger: 'blur' }
        ]
      },
      routerTree: [],
      routerTreeCheckedKeys: [],
      routerTreeDefaultSelectedKeys: [],
      columns: [
        {
          title: '名称',
          dataIndex: 'name'
        },
        {
          title: '操作',
          width: '150px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' }
        }
      ],
      actionTableSelectedRowKeys: [],
      actionTableSelectedRows: [],
      actionTableDatas: []
    }
  },
  created () {
    this.loadActions()
    this.loadRouterTreeList()
  },
  methods: {
    loadActions () {
      const datas = [{ id: 1, name: '新增' }, { id: 2, name: '修改' }, { id: 3, name: '查询' }, { id: 4, name: '删除' }]
      this.actionTableDatas = datas
    },
    loadRouterTreeList () {
      getPermissionTree().then((res) => {
         this.routerTree = res.data[0].children
        })
    },
    add () {
      this.edit({ id: 0 })
    },
    edit (record) {
      this.form = Object.assign({}, record)
      this.visible = true

      // router
      this.routerTreeCheckedKeys = record.routers.map(item => item.id)
      if (this.routerTreeCheckedKeys && this.routerTreeCheckedKeys.length > 0) {
        this.routerTreeDefaultSelectedKeys.push(this.routerTreeCheckedKeys[0])
        this.actionTableSelectedRowKeys = [2, 4]
      }
    },
    close () {
      this.$emit('close')
      this.visible = false
    },
    handleOk () {
      const _this = this
      this.$refs.ruleForm.validate(valid => {
         _this.confirmLoading = true
        if (valid) {
          _this.confirmLoading = false
          _this.close()
        }
      })
    },
    handleCancel () {
      this.close()
    },
    routerTreeHandleClick (e, node) {
      // todo
      this.actionTableSelectedRowKeys = [1, 2, 4]
    },
    routerTreeHandleCheck (checkedKeys, e) {
      var list = []
      this.routerTreeFindChildrenList(this.routerTree, e.node.value, false, list)
      var checkedList = this.routerTreeCheckedKeys.checked
      list.map(item => {
          if (e.checked) {
            if (checkedList.indexOf(item.key) === -1) {
              checkedList.push(item.key)
            }
          } else {
            if (checkedList.indexOf(item.key) > -1) {
              checkedList.splice(checkedList.findIndex(r => r === item.key), 1)
            }
          }
      })
    },
    routerTreeFindChildrenList (tree, checkedId, isChildren, list) {
      tree.map(item => {
        var isFind = false
        if (!isChildren) {
          if (item.key === checkedId) {
            list.push(item)
            isFind = true
          }
        } else {
          if (item.parentId === checkedId) {
            list.push(item)
            isFind = true
          }
        }

        if (item.children && item.children.length > 0) {
          this.routerTreeFindChildrenList(item.children, item.key, isFind, list)
        }
      })
    },
    onActionTableSelectChange (selectedRowKeys, selectedRows) {
      this.actionTableSelectedRowKeys = selectedRowKeys
      this.actionTableSelectedRows = selectedRows
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

  }
}
</script>

<style scoped>

</style>
