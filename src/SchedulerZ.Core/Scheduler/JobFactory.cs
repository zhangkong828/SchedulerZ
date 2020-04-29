using SchedulerZ.Core.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SchedulerZ.Core.Scheduler
{
    public class JobFactory
    {
        private const string DefaultJobBaseAssemblyName = "SchedulerZ";

        public static JobRuntime CreateJobRuntime(JobView jobView)
        {
            var domain = DomainManager.Create(jobView.Id);

            DeleteJobBaseAssemblyFromOutput(jobView.Id);
            var assemblyPath = JobFactory.GetJobAssemblyPath(jobView.Id, jobView.AssemblyName);
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

        private static void DeleteJobBaseAssemblyFromOutput(string jobId)
        {
            var dllPath = $"{Directory.GetCurrentDirectory()}\\Jobs\\{jobId}\\{DefaultJobBaseAssemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
            if (File.Exists(dllPath))
                File.Delete(dllPath);

            var pdbPath = $"{Directory.GetCurrentDirectory()}\\Jobs\\{jobId}\\{DefaultJobBaseAssemblyName}.pdb".Replace('\\', Path.DirectorySeparatorChar);
            if (File.Exists(pdbPath))
                File.Delete(pdbPath);
        }

        public static string GetJobAssemblyPath(string jobId, string assemblyName)
        {
            return $"{Directory.GetCurrentDirectory()}\\Jobs\\{jobId}\\{assemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }

    }
}
