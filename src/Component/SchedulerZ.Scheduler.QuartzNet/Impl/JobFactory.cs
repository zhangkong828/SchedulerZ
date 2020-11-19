using SchedulerZ.Domain;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobFactory
    {
        private const string DefaultJobBaseAssemblyName = "SchedulerZ";

        public static JobRuntime CreateJobRuntime(JobEntity jobView)
        {
            var assemblyPath = GetJobAssemblyPath(jobView);
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

        public static string GetJobAssemblyPath(JobEntity jobView)
        {
            //job解压缩后的目录
            var directoryPath = $"{Directory.GetCurrentDirectory()}\\{Config.Options.JobDirectory}\\src\\{jobView.Id}{jobView.Name}".Replace('\\', Path.DirectorySeparatorChar);
            if (!Directory.Exists(directoryPath))
            {
                var zipPath = $"{Directory.GetCurrentDirectory()}\\{Config.Options.JobDirectory}\\{jobView.FilePath}".Replace('\\', Path.DirectorySeparatorChar);
                if (!File.Exists(zipPath)) return null;
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
