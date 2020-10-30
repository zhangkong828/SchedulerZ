<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="48">
          <a-col :md="8" :sm="24">
            <a-form-item label="名称">
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
      <a-button type="primary" icon="plus" @click="$refs.modal.add()">新建</a-button>
    </div>

    <s-table
      ref="table"
      :columns="columns"
      :data="loadData"
      :expandedRowKeys="expandedRowKeys"
      row-key="id"
      @expand="handleExpand"
    >
      <div
        slot="expandedRowRender"
        slot-scope="record"
        style="margin: 0">
        <a-row
          :gutter="24"
          :style="{ marginBottom: '12px' }">
          <a-col :span="12" v-for="(router, index) in record.routers" :key="index" :style="{ marginBottom: '12px' }">
            <a-col :span="4">
              <span>{{ router.title }}：</span>
            </a-col>
            <a-col :span="20" v-if="router.permission">
              <a-tag color="cyan" v-for="(action, k) in router.permission.split(',')" :key="k">{{ action }}</a-tag>
            </a-col>
            <a-col :span="20" v-else>-</a-col>
          </a-col>
        </a-row>
      </div>
      <span slot="action" slot-scope="text, record">
        <a @click="$refs.modal.edit(record)">编辑</a>
        <a-divider type="vertical" />
        <a-popconfirm
          title="确定要删除?"
          ok-text="Yes"
          cancel-text="No"
          @confirm="handleDelete(record)"
        >
          <a href="#">删除</a>
        </a-popconfirm>
      </span>
    </s-table>

    <role-modal ref="modal" @ok="handleOk"></role-modal>

  </a-card>
</template>

<script>
import { STable } from '@/components'
import RoleModal from './modules/RoleModal'
import { getRoleList } from '@/api/system'

export default {
  name: 'TableList',
  components: {
    STable,
    RoleModal
  },
  data () {
    return {
      visible: false,
      form: null,
      mdl: {},
      // 查询参数
      queryParam: {},
      // 表头
      columns: [
        {
          title: '唯一识别码',
          dataIndex: 'identify'
        },
        {
          title: '角色名称',
          dataIndex: 'name'
        },
        {
          title: '备注',
          dataIndex: 'remark'
        },
        {
          title: '创建时间',
          dataIndex: 'createTime',
          sorter: true
        },
        {
          title: '操作',
          width: '150px',
          dataIndex: 'action',
          scopedSlots: { customRender: 'action' }
        }
      ],
      loadData: parameter => {
        const requestParameters = Object.assign({}, parameter, this.queryParam)
        return getRoleList(requestParameters).then((res) => {
          console.log('getRoleList', res)
          this.expandedRowKeys = res.data.list.map(item => item.id)
          res.data.list.map(item => {
            item.routers.map(router => { router.permission = '列表,新增,查询,修改,删除' })
          })
          return res.data
        })
      },
      expandedRowKeys: []
    }
  },
  methods: {
    handleExpand (expanded, record) {
      if (expanded) {
        this.expandedRowKeys.push(record.id)
      } else {
        this.expandedRowKeys = this.expandedRowKeys.filter(item => record.id !== item)
      }
    },
    handleDelete (record) {},
    handleOk () {
      console.log('refresh')
      // 新增/修改 成功时，重载列表
      this.$refs.table.refresh()
    }
  }
}
</script>
