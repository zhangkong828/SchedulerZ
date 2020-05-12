using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SchedulerZ.Utility
{
    public static class IPHelper
    {
        /// <summary>
        /// A类: 10.0.0.0-10.255.255.255
        /// </summary>
        static private long aBegin, aEnd;
        /// <summary>
        /// B类: 172.16.0.0-172.31.255.255   
        /// </summary>
        static private long bBegin, bEnd;
        /// <summary>
        /// C类: 192.168.0.0-192.168.255.255
        /// </summary>
        static private long cBegin, cEnd;

        static IPHelper()
        {
            aBegin = GetIpNum("10.0.0.0");
            aEnd = GetIpNum("10.255.255.255");
            bBegin = GetIpNum("172.16.0.0");
            bEnd = GetIpNum("172.31.255.255");
            cBegin = GetIpNum("192.168.0.0");
            cEnd = GetIpNum("192.168.255.255");
        }

        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        public static IPAddress GetLocalIntranetIP()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var child in list)
            {
                if (IsIntranetIP(child)) return child;
            }

            return null;
        }

        /// <summary>
        /// 获取本机内网IP列表
        /// </summary>
        public static List<IPAddress> GetLocalIntranetIPList()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            var result = new List<IPAddress>();
            foreach (var child in list)
            {
                if (IsIntranetIP(child)) result.Add(child);
            }

            return result;
        }

        /// <summary>
        /// 判断是否内网IP
        /// </summary>
        public static bool IsIntranetIP(IPAddress ipAddress)
        {
            bool isInnerIp;
            var ipNum = GetIpNum(ipAddress);

            isInnerIp = IsIntranet(ipNum, aBegin, aEnd) || IsIntranet(ipNum, bBegin, bEnd) || IsIntranet(ipNum, cBegin, cEnd);
            return isInnerIp;
        }

        /// <summary>
        /// 判断是否内网IP
        /// </summary>
        public static bool IsIntranetIP(string ipAddress)
        {
            bool isInnerIp;
            var ipNum = GetIpNum(ipAddress);

            isInnerIp = IsIntranet(ipNum, aBegin, aEnd) || IsIntranet(ipNum, bBegin, bEnd) || IsIntranet(ipNum, cBegin, cEnd);
            return isInnerIp;
        }


        private static long GetIpNum(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
        }

        private static long GetIpNum(string ipAddress)
        {
            var ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);

            var ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
        }

        private static bool IsIntranet(long userIp, long begin, long end)
        {
            return userIp >= begin && userIp <= end;
        }
    }
}
