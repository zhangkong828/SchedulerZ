using SchedulerZ.Domain;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobFactory
    {
        private const string DefaultJobBaseAssemblyName = "SchedulerZ";

        public static JobRuntime CreateJobRuntime(string jobDirectory, JobEntity jobView)
        {
            var domain = DomainManager.Create(jobView.Id);

            DeleteJobBaseAssemblyFromOutput(jobDirectory, jobView);
            var assemblyPath = JobFactory.GetJobAssemblyPath(jobDirectory, jobView);
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

        private static void DeleteJobBaseAssemblyFromOutput(string jobDirectory, JobEntity jobView)
        {
            var dllPath = $"{Directory.GetCurrentDirectory()}\\Jobs\\src\\{jobView.AssemblyName}\\{DefaultJobBaseAssemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
            if (File.Exists(dllPath))
                File.Delete(dllPath);

            var pdbPath = $"{Directory.GetCurrentDirectory()}\\Jobs\\src\\{jobView.AssemblyName}\\{DefaultJobBaseAssemblyName}.pdb".Replace('\\', Path.DirectorySeparatorChar);
            if (File.Exists(pdbPath))
                File.Delete(pdbPath);
        }

        public static string GetJobAssemblyPath(string jobDirectory, JobEntity jobView)
        {
            return $"{Directory.GetCurrentDirectory()}\\Jobs\\src\\{jobView.AssemblyName}\\{jobView.AssemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }

    }
}
