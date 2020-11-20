﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SchedulerZ
{
    public static class PathHelper
    {
        /// <summary>
        /// 基础目录。GetBasePath依赖于此，默认为当前应用程序域基础目录。用于X组件内部各目录，专门为函数计算而定制</summary>
        /// <remarks>
        /// 为了适应函数计算，该路径将支持从命令行参数和环境变量读取
        /// </remarks>
        public static string BasePath { get; set; }

        static PathHelper()
        {
            var dir = "";
            // 命令参数
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].EqualIgnoreCase("-BasePath", "--BasePath") && i + 1 < args.Length)
                {
                    dir = args[i + 1];
                    break;
                }
            }

            // 环境变量
            if (dir.IsNullOrEmpty()) dir = Environment.GetEnvironmentVariable("BasePath");

            // 最终取应用程序域。Linux下编译为单文件时，应用程序释放到临时目录，应用程序域基路径不对，当前目录也不一定正确，唯有进程路径正确
            if (dir.IsNullOrEmpty()) dir = AppDomain.CurrentDomain.BaseDirectory;

            BasePath = GetPath(dir, 1);
        }

        private static string GetPath(string path, int mode)
        {
            // 处理路径分隔符，兼容Windows和Linux
            var sep = Path.DirectorySeparatorChar;
            var sep2 = sep == '/' ? '\\' : '/';
            path = path.Replace(sep2, sep);

            var dir = "";
            switch (mode)
            {
                case 1:
                    dir = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                case 2:
                    dir = BasePath;
                    break;
                case 3:
                    dir = Environment.CurrentDirectory;
                    break;
                default:
                    break;
            }
            if (dir.IsNullOrEmpty()) return Path.GetFullPath(path);

            // 处理网络路径
            if (path.StartsWith(@"\\", StringComparison.Ordinal)) return Path.GetFullPath(path);

            // 考虑兼容Linux
            if (!Runtime.Mono)
            {
                //if (!Path.IsPathRooted(path))
                //!!! 注意：不能直接依赖于Path.IsPathRooted判断，/和\开头的路径虽然是绝对路径，但是它们不是驱动器级别的绝对路径
                if (/*path[0] == sep ||*/ path[0] == sep2 || !Path.IsPathRooted(path))
                {
                    path = path.TrimStart('~');

                    path = path.TrimStart(sep);
                    path = Path.Combine(dir, path);
                }
            }
            else
            {
                if (!path.StartsWith(dir))
                {
                    // path目录存在，不用再次拼接
                    if (!Directory.Exists(path))
                    {
                        path = path.TrimStart(sep);
                        path = Path.Combine(dir, path);
                    }
                }
            }

            return Path.GetFullPath(path);
        }

        /// <summary>
        /// 获取文件或目录基于应用程序域基目录的全路径，过滤相对目录
        /// </summary>
        /// <remarks>不确保目录后面一定有分隔符，是否有分隔符由原始路径末尾决定</remarks>
        /// <param name="path">文件或目录</param>
        /// <returns></returns>
        public static string GetFullPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            return GetPath(path, 1);
        }

        /// <summary>
        /// 获取文件或目录的全路径，过滤相对目录。用于X组件内部各目录，专门为函数计算而定制
        /// </summary>
        /// <remarks>不确保目录后面一定有分隔符，是否有分隔符由原始路径末尾决定</remarks>
        /// <param name="path">文件或目录</param>
        /// <returns></returns>
        public static string GetBasePath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            return GetPath(path, 2);
        }

        /// <summary>
        /// 确保目录存在，若不存在则创建
        /// </summary>
        /// <remarks>
        /// 斜杠结尾的路径一定是目录，无视第二参数；
        /// 默认是文件，这样子只需要确保上一层目录存在即可，否则如果把文件当成了目录，目录的创建会导致文件无法创建。
        /// </remarks>
        /// <param name="path">文件路径或目录路径，斜杠结尾的路径一定是目录，无视第二参数</param>
        /// <param name="isfile">该路径是否是否文件路径。文件路径需要取目录部分</param>
        /// <returns></returns>
        public static string EnsureDirectory(this string path, bool isfile = true)
        {
            if (string.IsNullOrEmpty(path)) return path;

            path = path.GetFullPath();
            if (File.Exists(path) || Directory.Exists(path)) return path;

            var dir = path;
            // 斜杠结尾的路径一定是目录，无视第二参数
            if (dir[dir.Length - 1] == Path.DirectorySeparatorChar)
                dir = Path.GetDirectoryName(path);
            else if (isfile)
                dir = Path.GetDirectoryName(path);

            /*!!! 基础类库的用法应该有明确的用途，而不是通过某些小伎俩去让人猜测 !!!*/

            //// 如果有圆点说明可能是文件
            //var p1 = dir.LastIndexOf('.');
            //if (p1 >= 0)
            //{
            //    // 要么没有斜杠，要么圆点必须在最后一个斜杠后面
            //    var p2 = dir.LastIndexOf('\\');
            //    if (p2 < 0 || p2 < p1) dir = Path.GetDirectoryName(path);
            //}

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return path;
        }

        /// <summary>合并多段路径</summary>
        /// <param name="path"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static string CombinePath(this string path, params string[] ps)
        {
            if (ps == null || ps.Length < 1) return path;
            if (path == null) path = string.Empty;

            //return Path.Combine(path, path2);
            foreach (var item in ps)
            {
                if (!item.IsNullOrEmpty()) path = Path.Combine(path, item);
            }
            return path;
        }

        /// <summary>路径作为目录信息</summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo AsDirectory(this string dir) => new DirectoryInfo(dir.GetFullPath());


        /// <summary>获取目录内所有符合条件的文件，支持多文件扩展匹配</summary>
        /// <param name="di">目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo di, string exts = null, bool allSub = false)
        {
            if (di == null || !di.Exists) yield break;

            if (string.IsNullOrEmpty(exts)) exts = "*";
            var opt = allSub ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (var pattern in exts.Split(new string[] { ";", "|", "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var item in di.GetFiles(pattern, opt))
                {
                    yield return item;
                }
            }
        }

        /// <summary>文件路径作为文件信息</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileInfo AsFile(this string file) => new FileInfo(file.GetFullPath());

        
        /// <summary>把数据写入文件指定位置</summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static FileInfo WriteBytes(this FileInfo file, byte[] data, int offset = 0)
        {
            using (var fs = file.OpenWrite())
            {
                fs.Position = offset;

                fs.Write(data, offset, data.Length);
            }

            return file;
        }

        /// <summary>读取所有文本，自动检测编码</summary>
        /// <remarks>性能较File.ReadAllText略慢，可通过提前检测BOM编码来优化</remarks>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String ReadText(this FileInfo file, Encoding encoding = null)
        {
            using var fs = file.OpenRead();
            if (encoding == null) encoding = Encoding.UTF8;
            using var reader = new StreamReader(fs, encoding);
            return reader.ReadToEnd();
        }

        /// <summary>把文本写入文件，自动检测编码</summary>
        /// <param name="file"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static FileInfo WriteText(this FileInfo file, string text, Encoding encoding = null)
        {
            using var fs = file.OpenWrite();
            if (encoding == null) encoding = Encoding.UTF8;
            using var writer = new StreamWriter(fs, encoding);
            writer.Write(text);

            return file;
        }


        /// <summary>复制目录中的文件</summary>
        /// <param name="di">源目录</param>
        /// <param name="destDirName">目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <param name="callback">复制每一个文件之前的回调</param>
        /// <returns></returns>
        public static string[] CopyTo(this DirectoryInfo di, string destDirName, string exts = null, bool allSub = false, Action<string> callback = null)
        {
            if (!di.Exists) return new string[0];

            var list = new List<string>();

            // 来源目录根，用于截断
            var root = di.FullName.EnsureEnd(Path.DirectorySeparatorChar.ToString());
            foreach (var item in di.GetAllFiles(exts, allSub))
            {
                var name = item.FullName.TrimStart(root);
                var dst = destDirName.CombinePath(name);
                callback?.Invoke(name);
                item.CopyTo(dst.EnsureDirectory(true), true);

                list.Add(dst);
            }

            return list.ToArray();
        }

        /// <summary>对比源目录和目标目录，复制双方都存在且源目录较新的文件</summary>
        /// <param name="di">源目录</param>
        /// <param name="destDirName">目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <param name="callback">复制每一个文件之前的回调</param>
        /// <returns></returns>
        public static string[] CopyToIfNewer(this DirectoryInfo di, string destDirName, string exts = null, bool allSub = false, Action<string> callback = null)
        {
            var dest = destDirName.AsDirectory();
            if (!dest.Exists) return new string[0];

            var list = new List<string>();

            // 目标目录根，用于截断
            var root = dest.FullName.EnsureEnd(Path.DirectorySeparatorChar.ToString());
            // 遍历目标目录，拷贝同名文件
            foreach (var item in dest.GetAllFiles(exts, allSub))
            {
                var name = item.FullName.TrimStart(root);
                var fi = di.FullName.CombinePath(name).AsFile();
                //fi.CopyToIfNewer(item.FullName);
                if (fi.Exists && item.Exists && fi.LastWriteTime > item.LastWriteTime)
                {
                    callback?.Invoke(name);
                    fi.CopyTo(item.FullName, true);
                    list.Add(fi.FullName);
                }
            }

            return list.ToArray();
        }

        /// <summary>从多个目标目录复制较新文件到当前目录</summary>
        /// <param name="di">当前目录</param>
        /// <param name="source">多个目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <returns></returns>
        public static string[] CopyIfNewer(this DirectoryInfo di, string[] source, string exts = null, bool allSub = false)
        {
            var list = new List<string>();
            var cur = di.FullName;
            foreach (var item in source)
            {
                // 跳过当前目录
                if (item.GetFullPath().EqualIgnoreCase(cur)) continue;

                Console.WriteLine("复制 {0} => {1}", item, cur);

                try
                {
                    var rs = item.AsDirectory().CopyToIfNewer(cur, exts, allSub, name =>
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t{1}\t{0}", name, item.CombinePath(name).AsFile().LastWriteTime.ToFullString());
                        Console.ResetColor();
                    });
                    if (rs != null && rs.Length > 0) list.AddRange(rs);
                }
                catch (Exception ex) { Console.WriteLine(" " + ex.Message); }
            }

            return list.ToArray();
        }
    }
}
