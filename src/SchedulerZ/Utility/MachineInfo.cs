﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ
{
    /// <summary>机器信息</summary>
    public class MachineInfo
    {
        #region 属性
        /// <summary>系统名称</summary>
        public String OSName { get; set; }

        /// <summary>系统版本</summary>
        public String OSVersion { get; set; }

        /// <summary>产品名称。制造商</summary>
        public String Product { get; set; }

        /// <summary>处理器型号</summary>
        public String Processor { get; set; }

        /// <summary>处理器序列号</summary>
        public String CpuID { get; set; }

        /// <summary>硬件唯一标识</summary>
        public String UUID { get; set; }

        /// <summary>系统标识</summary>
        public String Guid { get; set; }

        /// <summary>磁盘序列号</summary>
        public String DiskID { get; set; }

        /// <summary>内存总量</summary>
        public UInt64 Memory { get; set; }

        /// <summary>可用内存</summary>
        public UInt64 AvailableMemory { get; private set; }

        /// <summary>CPU占用率</summary>
        public Single CpuRate { get; private set; }

        /// <summary>温度</summary>
        public Double Temperature { get; set; }
        #endregion

        #region 构造
        /// <summary>当前机器信息。默认null，在RegisterAsync后才能使用</summary>
        public static MachineInfo Current { get; set; }

        private static Task<MachineInfo> _task;
        /// <summary>异步注册一个初始化后的机器信息实例</summary>
        /// <returns></returns>
        public static Task<MachineInfo> RegisterAsync()
        {
            if (_task != null) return _task;

            return _task = Task.Factory.StartNew(() =>
            {
                //var dataPath = "Data";

                //// 文件缓存，加快机器信息获取
                //var file = Path.GetTempPath().CombinePath("machine_info.json");
                //var file2 = dataPath.CombinePath("machine_info.json").GetBasePath();
                //if (Current == null)
                //{
                //    var f = file;
                //    if (!File.Exists(f)) f = file2;
                //    if (File.Exists(f))
                //    {
                //        try
                //        {
                //            //XTrace.WriteLine("Load MachineInfo {0}", f);
                //            Current = File.ReadAllText(f).ToJsonEntity<MachineInfo>();
                //        }
                //        catch { }
                //    }
                //}

                var mi = Current ?? new MachineInfo();

                mi.Init();
                Current = mi;

                //// 定时刷新
                //if (msRefresh > 0) mi._timer = new TimerX(s => mi.Refresh(), null, msRefresh, msRefresh) { Async = true };
                
                try
                {
                    //var json = mi.ToJson(true);
                    //File.WriteAllText(file.EnsureDirectory(true), json);
                    //File.WriteAllText(file2.EnsureDirectory(true), json);
                }
                catch { }

                return mi;
            });
        }

        /// <summary>获取当前信息，如果未设置则等待异步注册结果</summary>
        /// <returns></returns>
        public static MachineInfo GetCurrent() => Current ?? RegisterAsync().Result;

        #endregion

        #region 方法
        /// <summary>刷新</summary>
        public void Init()
        {
            var osv = Environment.OSVersion;
            if (OSVersion.IsNullOrEmpty()) OSVersion = osv.Version + "";
            if (OSName.IsNullOrEmpty()) OSName = (osv + "").TrimStart("Microsoft").TrimEnd(OSVersion).Trim();
            if (Guid.IsNullOrEmpty()) Guid = "";

            try
            {
                if (Runtime.Windows)
                    LoadWindowsInfo();
                else if (Runtime.Linux)
                    LoadLinuxInfo();
            }
            catch { }

            // window+netcore 不方便读取注册表，随机生成一个guid，借助文件缓存确保其不变
            if (Guid.IsNullOrEmpty()) Guid = "0-" + System.Guid.NewGuid().ToString();
            if (UUID.IsNullOrEmpty()) UUID = "0-" + System.Guid.NewGuid().ToString();

            try
            {
                Refresh();
            }
            catch { }
        }

        private void LoadWindowsInfo()
        {
            var str = "";

            var os = ReadWmic("os", "Caption", "Version");
            if (os != null)
            {
                if (os.TryGetValue("Caption", out str)) OSName = str.TrimStart("Microsoft").Trim();
                if (os.TryGetValue("Version", out str)) OSVersion = str;
            }

            var csproduct = ReadWmic("csproduct", "Name", "UUID");
            if (csproduct != null)
            {
                if (csproduct.TryGetValue("Name", out str)) Product = str;
                if (csproduct.TryGetValue("UUID", out str)) UUID = str;
            }

            var disk = ReadWmic("diskdrive", "serialnumber");
            if (disk != null)
            {
                if (disk.TryGetValue("serialnumber", out str)) DiskID = str?.Trim();
            }

            // 不要在刷新里面取CPU负载，因为运行wmic会导致CPU负载很不准确，影响测量
            var cpu = ReadWmic("cpu", "Name", "ProcessorId", "LoadPercentage");
            if (cpu != null)
            {
                if (cpu.TryGetValue("Name", out str)) Processor = str;
                if (cpu.TryGetValue("ProcessorId", out str)) CpuID = str;
                if (cpu.TryGetValue("LoadPercentage", out str)) CpuRate = (float)(str.ToDouble() / 100);
            }

            // 从注册表读取 MachineGuid
            str = Execute("reg", @"query HKLM\SOFTWARE\Microsoft\Cryptography /v MachineGuid");
            if (!str.IsNullOrEmpty() && str.Contains("REG_SZ")) Guid = str.Substring("REG_SZ", null).Trim();
        }

        private void LoadLinuxInfo()
        {
            var str = GetLinuxName();
            if (!str.IsNullOrEmpty()) OSName = str;

            // 树莓派的Hardware无法区分P0/P4
            var dic = ReadInfo("/proc/cpuinfo");
            if (dic != null)
            {
                if (dic.TryGetValue("Hardware", out str) ||
                    dic.TryGetValue("cpu model", out str) ||
                    dic.TryGetValue("model name", out str))
                    Processor = str;

                if (dic.TryGetValue("Model", out str)) Product = str;
                if (dic.TryGetValue("Serial", out str)) CpuID = str;
            }

            var mid = "/etc/machine-id";
            if (!File.Exists(mid)) mid = "/var/lib/dbus/machine-id";
            if (File.Exists(mid)) Guid = File.ReadAllText(mid).Trim();

            var file = "/sys/class/dmi/id/product_uuid";
            if (File.Exists(file)) UUID = File.ReadAllText(file).Trim();
            file = "/sys/class/dmi/id/product_name";
            if (File.Exists(file)) Product = File.ReadAllText(file).Trim();

            var disks = GetFiles("/dev/disk/by-id", true);
            if (disks.Count == 0) disks = GetFiles("/dev/disk/by-uuid", false);
            if (disks.Count > 0) DiskID = string.Join(",", disks);

            var dmi = Execute("dmidecode")?.SplitAsDictionary(":", "\n");
            if (dmi != null)
            {
                if (dmi.TryGetValue("ID", out str)) CpuID = str.Replace(" ", null);
                if (dmi.TryGetValue("UUID", out str)) UUID = str;
                if (dmi.TryGetValue("Product Name", out str)) Product = str;
                //if (TryFind(dmi, new[] { "Serial Number" }, out str)) Guid = str;
            }

            // 从release文件读取产品
            var prd = GetProductByRelease();
            if (!prd.IsNullOrEmpty()) Product = prd;
        }

        /// <summary>获取实时数据，如CPU、内存、温度</summary>
        public void Refresh()
        {
            if (Runtime.Windows)
            {
                MEMORYSTATUSEX ms = default;
                ms.Init();
                if (GlobalMemoryStatusEx(ref ms))
                {
                    Memory = ms.ullTotalPhys;
                    AvailableMemory = ms.ullAvailPhys;
                }
            }
            // 特别识别Linux发行版
            else if (Runtime.Linux)
            {
                var dic = ReadInfo("/proc/meminfo");
                if (dic != null)
                {
                    if (dic.TryGetValue("MemTotal", out var str))
                        Memory = (UInt64)str.TrimEnd(" kB").ToInt() * 1024;

                    if (dic.TryGetValue("MemAvailable", out str) ||
                        dic.TryGetValue("MemFree", out str))
                        AvailableMemory = (UInt64)str.TrimEnd(" kB").ToInt() * 1024;
                }

                var file = "/sys/class/thermal/thermal_zone0/temp";
                if (File.Exists(file))
                    Temperature = File.ReadAllText(file).Trim().ToDouble() / 1000;
                else
                {
                    // A2温度获取，Ubuntu 16.04 LTS， Linux 3.4.39
                    file = "/sys/class/hwmon/hwmon0/device/temp_value";
                    if (File.Exists(file)) Temperature = File.ReadAllText(file).Trim().Substring(null, ":").ToDouble();
                }

                //var upt = Execute("uptime");
                //if (!upt.IsNullOrEmpty())
                //{
                //    str = upt.Substring("load average:");
                //    if (!str.IsNullOrEmpty()) CpuRate = (Single)str.Split(",")[0].ToDouble();
                //}

                //file = "/proc/loadavg";
                //if (File.Exists(file)) CpuRate = (Single)File.ReadAllText(file).Substring(null, " ").ToDouble() / Environment.ProcessorCount;

                file = "/proc/stat";
                if (File.Exists(file))
                {
                    // CPU指标：user，nice, system, idle, iowait, irq, softirq
                    // cpu  57057 0 14420 1554816 0 443 0 0 0 0

                    using var reader = new StreamReader(file);
                    var line = reader.ReadLine();
                    if (!line.IsNullOrEmpty() && line.StartsWith("cpu"))
                    {
                        var vs = line.TrimStart("cpu").Trim().Split(" ");
                        var current = new SystemTime
                        {
                            IdleTime = vs[3].ToLong(),
                            TotalTime = vs.Take(7).Select(e => e.ToLong()).Sum().ToLong(),
                        };

                        var idle = current.IdleTime - (_systemTime?.IdleTime ?? 0);
                        var total = current.TotalTime - (_systemTime?.TotalTime ?? 0);
                        _systemTime = current;

                        CpuRate = total == 0 ? 0 : ((Single)(total - idle) / total);
                    }
                }
            }

            if (Runtime.Windows)
            {
                GetSystemTimes(out var idleTime, out var kernelTime, out var userTime);

                var current = new SystemTime
                {
                    IdleTime = idleTime.ToLong(),
                    TotalTime = kernelTime.ToLong() + userTime.ToLong(),
                };

                var idle = current.IdleTime - (_systemTime?.IdleTime ?? 0);
                var total = current.TotalTime - (_systemTime?.TotalTime ?? 0);
                _systemTime = current;

                CpuRate = total == 0 ? 0 : ((Single)(total - idle) / total);
            }
        }
        #endregion

        #region 辅助
        /// <summary>获取Linux发行版名称</summary>
        /// <returns></returns>
        public static String GetLinuxName()
        {
            var fr = "/etc/redhat-release";
            if (File.Exists(fr)) return File.ReadAllText(fr).Trim();

            var dr = "/etc/debian-release";
            if (File.Exists(dr)) return File.ReadAllText(dr).Trim();

            var sr = "/etc/os-release";
            if (File.Exists(sr)) return File.ReadAllText(sr).SplitAsDictionary("=", "\n", true)["PRETTY_NAME"].Trim();

            var uname = Execute("uname", "-sr")?.Trim();
            if (!uname.IsNullOrEmpty()) return uname;

            return null;
        }

        private static String GetProductByRelease()
        {
            var di = "/etc/".AsDirectory();
            if (!di.Exists) return null;

            foreach (var fi in di.GetFiles("*-release"))
            {
                if (!fi.Name.EqualIgnoreCase("redhat-release", "debian-release", "os-release", "system-release"))
                {
                    var dic = File.ReadAllText(fi.FullName).SplitAsDictionary("=", "\n", true);
                    if (dic.TryGetValue("BOARD", out var str)) return str;
                    if (dic.TryGetValue("BOARD_NAME", out str)) return str;
                }
            }

            return null;
        }

        private static IDictionary<String, String> ReadInfo(String file, Char separate = ':')
        {
            if (file.IsNullOrEmpty() || !File.Exists(file)) return null;

            var dic = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

            using var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                // 按行读取
                var line = reader.ReadLine();
                if (line != null)
                {
                    // 分割
                    var p = line.IndexOf(separate);
                    if (p > 0)
                    {
                        var key = line.Substring(0, p).Trim();
                        var value = line.Substring(p + 1).Trim();
                        dic[key] = value;
                    }
                }
            }

            return dic;
        }

        private static String Execute(String cmd, String arguments = null)
        {
            try
            {
                var psi = new ProcessStartInfo(cmd, arguments) { RedirectStandardOutput = true };
                var process = Process.Start(psi);
                if (!process.WaitForExit(3_000))
                {
                    process.Kill();
                    return null;
                }

                return process.StandardOutput.ReadToEnd();
            }
            catch { return null; }
        }

        /// <summary>通过WMIC命令读取信息</summary>
        /// <param name="type"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IDictionary<String, String> ReadWmic(String type, params String[] keys)
        {
            var dic = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

            var args = $"{type} get {string.Join(",",keys)} /format:list";
            var str = Execute("wmic", args)?.Trim();
            if (str.IsNullOrEmpty()) return dic;

            //return str.SplitAsDictionary("=", Environment.NewLine);

            var ss = str.Split(Environment.NewLine);
            foreach (var item in ss)
            {
                var ks = item.Split("=");
                if (ks != null && ks.Length >= 2)
                {
                    var k = ks[0].Trim();
                    var v = ks[1].Trim();
                    if (dic.TryGetValue(k, out var val))
                        dic[k] = val + "," + v;
                    else
                        dic[k] = v;
                }
            }

            return dic;
        }
        #endregion

        #region 内存
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SecurityCritical]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        internal struct MEMORYSTATUSEX
        {
            internal UInt32 dwLength;

            internal UInt32 dwMemoryLoad;

            internal UInt64 ullTotalPhys;

            internal UInt64 ullAvailPhys;

            internal UInt64 ullTotalPageFile;

            internal UInt64 ullAvailPageFile;

            internal UInt64 ullTotalVirtual;

            internal UInt64 ullAvailVirtual;

            internal UInt64 ullAvailExtendedVirtual;

            internal void Init() => dwLength = checked((UInt32)Marshal.SizeOf(typeof(MEMORYSTATUSEX)));
        }
        #endregion

        #region 磁盘
        /// <summary>获取指定目录所在盘可用空间，默认当前目录</summary>
        /// <param name="path"></param>
        /// <returns>返回可用空间，字节，获取失败返回-1</returns>
        public static Int64 GetFreeSpace(String path = null)
        {
            if (path.IsNullOrEmpty()) path = ".";

            var driveInfo = new DriveInfo(Path.GetPathRoot(path.GetFullPath()));
            if (driveInfo == null || !driveInfo.IsReady) return -1;

            try
            {
                return driveInfo.AvailableFreeSpace;
            }
            catch { return -1; }
        }

        /// <summary>获取指定目录下文件名，支持去掉后缀的去重，主要用于Linux</summary>
        /// <param name="path"></param>
        /// <param name="trimSuffix"></param>
        /// <returns></returns>
        public static ICollection<String> GetFiles(String path, Boolean trimSuffix = false)
        {
            var list = new List<String>();
            if (path.IsNullOrEmpty()) return list;

            var di = path.AsDirectory();
            if (!di.Exists) return list;

            var list2 = di.GetFiles().Select(e => e.Name).ToList();
            foreach (var item in list2)
            {
                var line = item?.Trim();
                if (!line.IsNullOrEmpty())
                {
                    if (trimSuffix)
                    {
                        if (!list2.Any(e => e != line && line.StartsWith(e))) list.Add(line);
                    }
                    else
                    {
                        list.Add(line);
                    }
                }
            }

            return list;
        }
        #endregion

        #region Windows辅助
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern Boolean GetSystemTimes(out FILETIME idleTime, out FILETIME kernelTime, out FILETIME userTime);

        private struct FILETIME
        {
            public UInt32 Low;

            public UInt32 High;

            public FILETIME(Int64 time)
            {
                Low = (UInt32)time;
                High = (UInt32)(time >> 32);
            }

            public Int64 ToLong() => (Int64)(((UInt64)High << 32) | Low);
        }

        private class SystemTime
        {
            public Int64 IdleTime;
            public Int64 TotalTime;
        }

        private SystemTime _systemTime;

        #endregion
    }
}
