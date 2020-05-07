using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerZ.Route
{
    public class ServiceRouteDescriptor
    {
        public ServiceRouteDescriptor()
        {
            Metadatas = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 元数据
        /// </summary> 
        public IDictionary<string, object> Metadatas { get; set; }

        /// <summary>
        /// 获取一个元数据
        /// </summary>
        /// <typeparam name="T">元数据类型</typeparam>
        /// <param name="name">元数据名称</param>
        /// <param name="def">如果指定名称的元数据不存在则返回这个参数</param>
        /// <returns>元数据值</returns>
        public T GetMetadata<T>(string name, T def = default(T))
        {
            if (!Metadatas.ContainsKey(name))
                return def;

            return (T)Metadatas[name];
        }


        public override bool Equals(object obj)
        {
            var model = obj as ServiceRouteDescriptor;
            if (model == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            if (model.Id != Id)
                return false;

            return model.Metadatas.Count == Metadatas.Count && model.Metadatas.All(metadata =>
            {
                object value;
                if (!Metadatas.TryGetValue(metadata.Key, out value))
                    return false;

                if (metadata.Value == null && value == null)
                    return true;
                if (metadata.Value == null || value == null)
                    return false;

                return metadata.Value.Equals(value);
            });
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(ServiceRouteDescriptor model1, ServiceRouteDescriptor model2)
        {
            return Equals(model1, model2);
        }

        public static bool operator !=(ServiceRouteDescriptor model1, ServiceRouteDescriptor model2)
        {
            return !Equals(model1, model2);
        }



    }
}
