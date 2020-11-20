using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerZ
{
    public static class stringHelper
    {
        /// <summary>
        /// 忽略大小写的字符串相等比较，判断是否以任意一个待比较字符串相等
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool EqualIgnoreCase(this string value, params string[] strs)
        {
            foreach (var item in strs)
            {
                if (string.Equals(value, item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// 忽略大小写的字符串开始比较，判断是否以任意一个待比较字符串开始
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string value, params string[] strs)
        {
            if (value == null || string.IsNullOrEmpty(value)) return false;

            foreach (var item in strs)
            {
                if (value.StartsWith(item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// 忽略大小写的字符串结束比较，判断是否以任意一个待比较字符串结束
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string value, params string[] strs)
        {
            if (value == null || string.IsNullOrEmpty(value)) return false;

            foreach (var item in strs)
            {
                if (value.EndsWith(item, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是 string.Empty 字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value) => value == null || value.Length <= 0;

        /// <summary>
        /// 是否空或者空白字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessage(this Exception ex)
        {
            var msg = ex + "";
            if (msg.IsNullOrEmpty()) return null;

            var ss = msg.Split(Environment.NewLine);
            var ns = ss.Where(e =>
            !e.StartsWith("---") &&
            !e.Contains("System.Runtime.ExceptionServices") &&
            !e.Contains("System.Runtime.CompilerServices"));

            msg = string.Join(Environment.NewLine, ns);

            return msg;
        }

        /// <summary>从当前字符串开头移除另一字符串，不区分大小写，循环多次匹配前缀</summary>
        /// <param name="str">当前字符串</param>
        /// <param name="starts">另一字符串</param>
        /// <returns></returns>
        public static string TrimStart(this string str, params string[] starts)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (starts == null || starts.Length < 1 || string.IsNullOrEmpty(starts[0])) return str;

            for (var i = 0; i < starts.Length; i++)
            {
                if (str.StartsWith(starts[i], StringComparison.OrdinalIgnoreCase))
                {
                    str = str.Substring(starts[i].Length);
                    if (string.IsNullOrEmpty(str)) break;

                    // 从头开始
                    i = -1;
                }
            }
            return str;
        }

        /// <summary>从当前字符串结尾移除另一字符串，不区分大小写，循环多次匹配后缀</summary>
        /// <param name="str">当前字符串</param>
        /// <param name="ends">另一字符串</param>
        /// <returns></returns>
        public static string TrimEnd(this string str, params string[] ends)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (ends == null || ends.Length < 1 || string.IsNullOrEmpty(ends[0])) return str;

            for (var i = 0; i < ends.Length; i++)
            {
                if (str.EndsWith(ends[i], StringComparison.OrdinalIgnoreCase))
                {
                    str = str.Substring(0, str.Length - ends[i].Length);
                    if (string.IsNullOrEmpty(str)) break;

                    // 从头开始
                    i = -1;
                }
            }
            return str;
        }

        /// <summary>确保字符串以指定的另一字符串开始，不区分大小写</summary>
        /// <param name="str">字符串</param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static string EnsureStart(this string str, string start)
        {
            if (string.IsNullOrEmpty(start)) return str;
            if (string.IsNullOrEmpty(str)) return start;

            if (str.StartsWith(start, StringComparison.OrdinalIgnoreCase)) return str;

            return start + str;
        }

        /// <summary>确保字符串以指定的另一字符串结束，不区分大小写</summary>
        /// <param name="str">字符串</param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string EnsureEnd(this string str, string end)
        {
            if (string.IsNullOrEmpty(end)) return str;
            if (string.IsNullOrEmpty(str)) return end;

            if (str.EndsWith(end, StringComparison.OrdinalIgnoreCase)) return str;

            return str + end;
        }

        /// <summary>从字符串中检索子字符串，在指定头部字符串之后，指定尾部字符串之前</summary>
        /// <remarks>常用于截取xml某一个元素等操作</remarks>
        /// <param name="str">目标字符串</param>
        /// <param name="after">头部字符串，在它之后</param>
        /// <param name="before">尾部字符串，在它之前</param>
        /// <param name="startIndex">搜索的开始位置</param>
        /// <param name="positions">位置数组，两个元素分别记录头尾位置</param>
        /// <returns></returns>
        public static string Substring(this string str, string after, string before = null, int startIndex = 0, int[] positions = null)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (string.IsNullOrEmpty(after) && string.IsNullOrEmpty(before)) return str;

            /*
             * 1，只有start，从该字符串之后部分
             * 2，只有end，从开头到该字符串之前
             * 3，同时start和end，取中间部分
             */

            var p = -1;
            if (!string.IsNullOrEmpty(after))
            {
                p = str.IndexOf(after, startIndex);
                if (p < 0) return null;
                p += after.Length;

                // 记录位置
                if (positions != null && positions.Length > 0) positions[0] = p;
            }

            if (string.IsNullOrEmpty(before)) return str.Substring(p);

            var f = str.IndexOf(before, p >= 0 ? p : startIndex);
            if (f < 0) return null;

            // 记录位置
            if (positions != null && positions.Length > 1) positions[1] = f;

            if (p >= 0)
                return str.Substring(p, f - p);
            else
                return str.Substring(0, f);
        }

        /// <summary>拆分字符串成为不区分大小写的可空名值字典。逗号分组，等号分隔</summary>
        /// <param name="value">字符串</param>
        /// <param name="nameValueSeparator">名值分隔符，默认等于号</param>
        /// <param name="separator">分组分隔符，默认分号</param>
        /// <param name="trimQuotation">去掉括号</param>
        /// <returns></returns>
        public static IDictionary<string, string> SplitAsDictionary(this string value, string nameValueSeparator = "=", string separator = ";", bool trimQuotation = false)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (value == null || value.IsNullOrWhiteSpace()) return dic;

            if (nameValueSeparator.IsNullOrEmpty()) nameValueSeparator = "=";
            //if (separator == null || separator.Length < 1) separator = new String[] { ",", ";" };

            var ss = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (ss == null || ss.Length < 1) return dic;

            foreach (var item in ss)
            {
                var p = item.IndexOf(nameValueSeparator);
                if (p <= 0) continue;

                var key = item.Substring(0, p).Trim();
                var val = item.Substring(p + nameValueSeparator.Length).Trim();

                // 处理单引号双引号
                if (trimQuotation && !val.IsNullOrEmpty())
                {
                    if (val[0] == '\'' && val[val.Length - 1] == '\'') val = val.Trim('\'');
                    if (val[0] == '"' && val[val.Length - 1] == '"') val = val.Trim('"');
                }

                dic[key] = val;
            }

            return dic;
        }
    }
}
