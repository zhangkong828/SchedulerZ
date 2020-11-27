using SchedulerZ.Domain;
using SchedulerZ.Models;
using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobFactory
    {
        private const string DefaultJobBaseAssemblyName = "SchedulerZ";

        public static async Task<JobRuntime> CreateJobRuntime(IServiceRoute serviceRoute, JobEntity jobView)
        {
            var assemblyPath = await GetJobAssemblyPath(serviceRoute, jobView);
            if (string.IsNullOrWhiteSpace(assemblyPath))
                throw new FileNotFoundException($"{jobView.FilePath}不存在");

            var domain = DomainManager.Create(jobView.Id);
            var assembly = domain.LoadFile(assemblyPath);

            Type type = assembly.GetType(jobView.ClassName, true, true);
            var instance = Activator.CreateInstance(type) as JobBase;
            if (instance == null)
            {
                throw new InvalidCastException($"程序集: {jobView.AssemblyName} 创建Job实例失败,请确认 {jobView.ClassName} 是否派生自JobBase");
            }

            return new JobRuntime()
            {
                JobView = jobView,
                Domain = domain,
                Instance = instance
            };
        }

        public static async Task<string> GetJobAssemblyPath(IServiceRoute serviceRoute, JobEntity jobView)
        {
            //job解压缩后的目录
            var directoryPath = $"{Directory.GetCurrentDirectory()}\\{Config.Options.JobDirectory}\\src\\{jobView.Id}{jobView.Name}".Replace('\\', Path.DirectorySeparatorChar);
            if (!Directory.Exists(directoryPath))
            {
                var zipPath = $"{Directory.GetCurrentDirectory()}\\{Config.Options.JobDirectory}\\{jobView.FilePath}".Replace('\\', Path.DirectorySeparatorChar);
                //下载
                if (!File.Exists(zipPath))
                {
                    var downloadResult = false;
                    var services = await serviceRoute.DiscoverServices("manager");
                    foreach (var service in services)
                    {
                        try
                        {
                            var url = $"http://{service.Address}:{service.Port}/Api/Packages/DownloadPackage?packageName={jobView.FilePath}";
                            using (var webClient = new WebClient())
                            {
                                await webClient.DownloadFileTaskAsync(url, zipPath);
                                downloadResult = true;
                                break;
                            }
                        }
                        catch { }
                    }
                    if (!downloadResult)
                    {
                        //下载失败 删除文件
                        if (File.Exists(zipPath)) File.Delete(zipPath);
                        return null;
                    }
                }
                //将指定zip解压缩到对应的目录下
                ZipFile.ExtractToDirectory(zipPath, directoryPath, true);
            }
            DeleteJobBaseAssemblyFromOutput(directoryPath);
            return $"{Directory.GetCurrentDirectory()}\\{Config.Options.JobDirectory}\\src\\{jobView.Id}{jobView.Name}\\{jobView.AssemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }

        private static void DeleteJobBaseAssemblyFromOutput(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath).Where(x => x.EndsWith(".pdb") || x.EndsWith($"{DefaultJobBaseAssemblyName}.dll"));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

    }
}
