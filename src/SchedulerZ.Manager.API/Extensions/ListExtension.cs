using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Extensions
{
    public static class ListExtension
    {
        public static List<T> ConvertToTree<T>(this List<T> list, object PId)
            where T : class, IConvertTree<T>
        {
            var tree = new List<T>();
            var tempList = list.Where(x => x.PId == PId).ToList();

            foreach (var item in tempList)
            {
                var treeNode = item.Clone();
                treeNode.AddChildrens(list.ConvertToTree<T>(item.GetId()));
                tree.Add(treeNode);
            }

            return tree;
        }
    }


    public interface IConvertTree<T> where T : class
    {
        object GetId();
        object PId { get; }
        void AddChildrens(List<T> childrens);
        T Clone();
    }
}
