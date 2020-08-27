﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Dto
{
    public class RouterDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 路径地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 路由名称 不能重复
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }

        public string Icon { get; set; }

        public bool HiddenHeaderContent { get; set; }

        /// <summary>
        /// 菜单链接跳转目标（参考 html a 标记） _blank|_self|_top|_parent
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 是否显示菜单
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// 是否隐藏子菜单
        /// </summary>
        public bool HideChildren { get; set; }

        /// <summary>
        /// 重定向地址, 访问这个路由时,自定进行重定向
        /// </summary>
        public string Redirect { get; set; }

        public string Remark { get; set; }

        public long ParentId { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }
    }
}
