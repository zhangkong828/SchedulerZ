using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace SchedulerZ.Logging
{
    public abstract class Logger : ILogger
    {
        /// <summary>
        /// 是否启用日志
        /// </summary>
        public virtual bool Enable { get; set; } = Config.LoggerOptions.Enable;

        /// <summary>
        /// 日志等级，只输出大于等于该级别的日志
        /// </summary>
        public virtual LogLevel Level { get; set; } = Config.LoggerOptions.LogLevel;


        public virtual void Write(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (Enable && level >= Level)
            {
                WriteLog(level, string.Format(format, args), ex);
            }
        }

        protected abstract void WriteLog(LogLevel level, string message, Exception ex);


        public void Trace(string message) => Write(LogLevel.Trace, null, message);
        public void Trace(string format, params object[] args) => Write(LogLevel.Trace, null, format, args);
        public void Trace(string message, Exception ex) => Write(LogLevel.Trace, ex, message);

        public void Debug(string message) => Write(LogLevel.Debug, null, message);
        public void Debug(string format, params object[] args) => Write(LogLevel.Debug, null, format, args);
        public void Debug(string message, Exception ex) => Write(LogLevel.Debug, ex, message);

        public void Info(string message) => Write(LogLevel.Info, null, message);
        public void Info(string format, params object[] args) => Write(LogLevel.Info, null, format, args);
        public void Info(string message, Exception ex) => Write(LogLevel.Info, ex, message);

        public void Error(string message) => Write(LogLevel.Error, null, message);
        public void Error(string format, params object[] args) => Write(LogLevel.Error, null, format, args);
        public void Error(string message, Exception ex) => Write(LogLevel.Error, ex, message);

        public void Fatal(string message) => Write(LogLevel.Fatal, null, message);
        public void Fatal(string format, params object[] args) => Write(LogLevel.Fatal, null, format, args);
        public void Fatal(string message, Exception ex) => Write(LogLevel.Fatal, ex, message);


        /// <summary>
        /// 输出日志头，包含所有环境信息
        /// </summary>
        protected static string GetHead()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var name = string.Empty;
            var ver = "";
            var asm = Assembly.GetEntryAssembly();
            if (asm != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    var att = asm.GetCustomAttribute<AssemblyTitleAttribute>();
                    if (att != null) name = att.Title;
                }

                if (string.IsNullOrEmpty(name))
                {
                    var att = asm.GetCustomAttribute<AssemblyProductAttribute>();
                    if (att != null) name = att.Product;
                }

                if (string.IsNullOrEmpty(name))
                {
                    var att = asm.GetCustomAttribute<AssemblyDescriptionAttribute>();
                    if (att != null) name = att.Description;
                }

                var tar = asm.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();
                if (tar != null) ver = tar.FrameworkDisplayName ?? tar.FrameworkName;
            }

            ver = RuntimeInformation.FrameworkDescription;

            if (string.IsNullOrEmpty(name))
            {
                try
                {
                    name = process.ProcessName;
                }
                catch { }
            }
            var sb = new StringBuilder();
            sb.AppendFormat("#Software: {0}\r\n", name);
            sb.AppendFormat("#ProcessID: {0}{1}\r\n", process.Id, Environment.Is64BitProcess ? " x64" : "");
            sb.AppendFormat("#AppDomain: {0}\r\n", AppDomain.CurrentDomain.FriendlyName);

            var fileName = string.Empty;
            // MonoAndroid无法识别MainModule，致命异常
            try
            {
                fileName = process.MainModule.FileName;
            }
            catch { }
            if (fileName.IsNullOrEmpty() || fileName.EndsWithIgnoreCase("dotnet", "dotnet.exe"))
            {
                try
                {
                    fileName = process.StartInfo.FileName;
                }
                catch { }
            }
            if (!fileName.IsNullOrEmpty()) sb.AppendFormat("#FileName: {0}\r\n", fileName);

            // 应用域目录
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            sb.AppendFormat("#BaseDirectory: {0}\r\n", baseDir);

            // 当前目录。如果由别的进程启动，默认的当前目录就是父级进程的当前目录
            var curDir = Environment.CurrentDirectory;
            //if (!curDir.EqualIC(baseDir) && !(curDir + "\\").EqualIC(baseDir))
            if (!baseDir.EqualIgnoreCase(curDir, curDir + "\\", curDir + "/"))
                sb.AppendFormat("#CurrentDirectory: {0}\r\n", curDir);

            var basePath = PathHelper.BasePath;
            if (basePath != baseDir)
                sb.AppendFormat("#BasePath: {0}\r\n", basePath);

            // 临时目录
            sb.AppendFormat("#TempPath: {0}\r\n", Path.GetTempPath());

            // 命令行不为空，也不是文件名时，才输出
            // 当使用cmd启动程序时，这里就是用户输入的整个命令行，所以可能包含空格和各种符号
            var line = Environment.CommandLine;
            if (!line.IsNullOrEmpty())
                sb.AppendFormat("#CommandLine: {0}\r\n", line);

            var apptype = "";
            if (!Environment.UserInteractive)
                apptype = "Service";
            else
                apptype = "Console";

            sb.AppendFormat("#ApplicationType: {0}\r\n", apptype);
            sb.AppendFormat("#CLR: {0}, {1}\r\n", Environment.Version, ver);

            var os = "";
            // 获取丰富的机器信息，需要提注册 MachineInfo.RegisterAsync
            var mi = MachineInfo.GetCurrent();
            if (mi != null)
            {
                os = mi.OSName + " " + mi.OSVersion;
            }
            else
            {
                // 特别识别Linux发行版
                os = Environment.OSVersion + "";
                if (Runtime.Linux) os = MachineInfo.GetLinuxName();
            }

            sb.AppendFormat("#OS: {0}, {1}/{2}\r\n", os, Environment.MachineName, Environment.UserName);
            sb.AppendFormat("#CPU: {0}\r\n", Environment.ProcessorCount);
            if (mi != null)
            {
                sb.AppendFormat("#Memory: {0:n0}M/{1:n0}M\r\n", mi.AvailableMemory / 1024 / 1024, mi.Memory / 1024 / 1024);
                sb.AppendFormat("#Processor: {0}\r\n", mi.Processor);
                if (!mi.Product.IsNullOrEmpty()) sb.AppendFormat("#Product: {0}\r\n", mi.Product);
                if (mi.Temperature > 0) sb.AppendFormat("#Temperature: {0}\r\n", mi.Temperature);
            }
            sb.AppendFormat("#GC: IsServerGC={0}, LatencyMode={1}\r\n", GCSettings.IsServerGC, GCSettings.LatencyMode);

            sb.AppendFormat("#Date: {0:yyyy-MM-dd}\r\n", DateTime.Now);
            sb.AppendFormat("#字段: 时间 线程ID 线程池Y/网页W/普通N/定时T 线程名/任务ID 消息内容\r\n");
            sb.AppendFormat("#Fields: Time ThreadID Kind Name Message\r\n");

            return sb.ToString();
        }
    }
}
