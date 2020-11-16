using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SchedulerZ.Utility
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
                var Types = s.GetType();//获得类型
                var Typed = typeof(Target);
                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name.ToLower() == sp.Name.ToLower())//判断属性名是否相同
                        {
                            if (dp.PropertyType == sp.PropertyType)
                                dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性
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

        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string JsonSerializeIgnoreNullValue(object obj)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(obj, jsetting);
        }

        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
