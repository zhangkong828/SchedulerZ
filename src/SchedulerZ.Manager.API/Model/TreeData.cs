using SchedulerZ.Manager.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class TreeData : IConvertTree<TreeData>
    {
        public TreeData()
        {
            Children = new List<TreeData>();
        }

        public string Title { get; set; }
        public object Value { get; set; }
        public object Key { get; set; }
        public object ParentId { get; set; }
        public List<TreeData> Children { get; set; }

        public object PId { get { return ParentId; } }

        public void AddChildrens(List<TreeData> childrens)
        {
            Children = childrens;
        }

        public TreeData Clone()
        {
            return new TreeData()
            {
                Title = this.Title,
                Value = this.Value,
                Key = this.Key,
                ParentId = this.ParentId
            };
        }

        public object GetId()
        {
            return Key;
        }

        public object GetParentId()
        {
            return ParentId;
        }
    }


}
