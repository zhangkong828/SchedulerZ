using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SchedulerZ.Utility
{
    public class Utils
    {
        public static Target MapperGrpcJob<Source, Target>(Source s)
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
                            else
                            {
                                if (dp.PropertyType.IsGenericType && dp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && sp.PropertyType.FullName == "System.Int64")
                                {
                                    var value = (long)sp.GetValue(s, null);
                                    if (value > 0)
                                    {
                                        var datetime = ConvertToDateTime(value);
                                        dp.SetValue(d, datetime, null);
                                    }
                                }
                                else if (dp.PropertyType.FullName == "System.Int64" && sp.PropertyType.IsGenericType && sp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var value = sp.GetValue(s, null);
                                    if (value != null)
                                    {
                                        if (DateTime.TryParse(value.ToString(), out DateTime datetime))
                                        {
                                            var timeStamp = ConvertToTimeStamp(datetime);
                                            dp.SetValue(d, timeStamp, null);
                                        }
                                    }
                                }
                            }
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

        /// <summary>
        /// 日期转换为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <param name="isMilliseconds">是否毫秒</param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime time, bool isMilliseconds = true)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            if (isMilliseconds)
            {
                return (int)(time - startTime).TotalMilliseconds;
            }
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 时间戳转换为日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="isMilliseconds">是否毫秒</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStamp, bool isMilliseconds = true)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            if (isMilliseconds)
            {
                return dtStart.AddMilliseconds(timeStamp);
            }
            else
            {
                return dtStart.AddSeconds(timeStamp);
            }
        }
    }
}
