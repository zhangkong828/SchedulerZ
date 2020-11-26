using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Utility
{
    public class Utils
    {
        /// <summary>
        /// 将属性名相同的原对象属性值映射到目标对象的属性值上
        /// </summary>
        /// <typeparam name="Source">原对象类型</typeparam>
        /// <typeparam name="Target">目标对象类型</typeparam>
        /// <param name="s">原对象</param>
        /// <returns></returns>
        public static Target MapperPropertyValue<Source, Target>(Source s)
        {
            Target d = Activator.CreateInstance<Target>();
            try
            {
                var Types = s.GetType();
                var Typed = typeof(Target);
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name.ToLower() == sp.Name.ToLower())
                        {
                            if (dp.PropertyType == sp.PropertyType)
                                dp.SetValue(d, sp.GetValue(s, null), null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        /// <summary>
        /// 集合批量操作，返回需要删除和新增的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        /// <param name="deleteList"></param>
        /// <param name="addList"></param>
        public static void ListBatchAddOrDelete<T>(List<T> oldList, List<T> newList, out List<T> deleteList, out List<T> addList)
        {
            deleteList = oldList.Except(newList).ToList();
            addList = newList.Except(oldList).ToList();
        }

    }
}
