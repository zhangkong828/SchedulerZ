using SchedulerZ.Manager.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class TreeData : IConvertTree<long,TreeData>
    {
        public TreeData()
        {
            Children = new List<TreeData>();
        }

        public string Title { get; set; }
        public object Value { get; set; }
        public long Key { get; set; }
        public long ParentId { get; set; }
        public List<TreeData> Children { get; set; }


        public void AddChildrens(List<TreeData> childrens)
        {
            if (childrens == null || childrens.Count == 0)
                Children = null;
            else
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

        public long GetId()
        {
            return Key;
        }

        public long GetPId()
        {
            return ParentId;
        }
    }


}
