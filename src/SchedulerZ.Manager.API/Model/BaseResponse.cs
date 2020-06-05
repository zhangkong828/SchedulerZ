using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class BaseResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Message { get; set; }

        public BaseResponse()
        {

        }

        public BaseResponse(int code) : this(code, "")
        {

        }

        public BaseResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public void SetBusinessStatus(ResponseStatusType code)
        {
            Code = (int)code;
            Message = GetEnumDescription(code);
        }

        public void SetBusinessStatus(ResponseStatusType code, string message)
        {
            Code = (int)code;
            Message = message;
        }

        public static BaseResponse GetBaseResponse(int code)
        {
            return new BaseResponse(code);
        }

        public static BaseResponse GetBaseResponse(int code, string message)
        {
            return new BaseResponse(code, message);
        }

        public static BaseResponse GetBaseResponse(ResponseStatusType code)
        {
            return new BaseResponse((int)code, GetEnumDescription(code));
        }

        public static BaseResponse GetBaseResponse(ResponseStatusType code, string message)
        {
            return new BaseResponse((int)code, message);
        }

        protected static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        public BaseResponse()
        {

        }

        public BaseResponse(int code) : base(code)
        {
        }

        public BaseResponse(int code, string message) : base(code, message)
        {
        }

        public BaseResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            if (data == null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                {
                    Data = Activator.CreateInstance<T>();
                }
                else
                {
                    Data = default(T);
                }
            }
            else
                Data = data;
        }

        public new static BaseResponse<T> GetBaseResponse(int code)
        {
            return new BaseResponse<T>(code);
        }

        public new static BaseResponse<T> GetBaseResponse(int code, string message)
        {
            return new BaseResponse<T>(code, message);
        }

        public static BaseResponse<T> GetBaseResponse(int code, string message, T data)
        {
            return new BaseResponse<T>(code, message, data);
        }

        public static BaseResponse<T> GetBaseResponse(ResponseStatusType code, T data)
        {
            return new BaseResponse<T>((int)code, GetEnumDescription(code), data);
        }

        public static BaseResponse<T> GetBaseResponse(ResponseStatusType code, string message, T data)
        {
            return new BaseResponse<T>((int)code, message, data);
        }

    }
}
