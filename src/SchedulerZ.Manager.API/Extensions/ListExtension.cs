using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Extensions
{
    public static class ListExtension
    {
        public static List<TNode> ConvertToTree<TId, TNode>(this List<TNode> list, TId PId)
            where TNode : class, IConvertTree<TId, TNode>
        {
            var tree = new List<TNode>();
            var tempList = list.Where(x => x.GetPId().Equals(PId)).ToList();

            foreach (var item in tempList)
            {
                var treeNode = item.Clone();
                treeNode.AddChildrens(list.ConvertToTree<TId, TNode>(item.GetId()));
                tree.Add(treeNode);
            }

            return tree;
        }
    }


    public interface IConvertTree<TId,TNode> where TNode : class
    {
        TId GetId();
        TId GetPId();
        void AddChildrens(List<TNode> childrens);
        TNode Clone();
    }
}
