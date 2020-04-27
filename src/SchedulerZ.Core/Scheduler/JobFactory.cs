using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SchedulerZ.Core.Scheduler
{
    public class JobFactory
    {
        public static JobBase CreateJobInstance(JobAssemblyLoadContext context, string jobId, string assemblyName, string className)
        {
            try
            {
                string jobLocation = GetJobAssemblyPath(assemblyName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(jobLocation)));
                Type type = assembly.GetType(className, true, true);
                var instance = Activator.CreateInstance(type);
                var j = instance as JobBase;
                return j;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetJobAssemblyPath(string assemblyName)
        {
            return $"{Directory.GetCurrentDirectory()}\\Jobs\\test\\{assemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }


        /// <summary>
        /// 加载应用程序域
        /// </summary>
        public static JobAssemblyLoadContext LoadAssemblyContext(string assemblyName)
        {
            string jobLocation = GetJobAssemblyPath(assemblyName);
            JobAssemblyLoadContext loadContext = new JobAssemblyLoadContext(jobLocation);
            return loadContext;
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        public static void UnLoadAssemblyContext(JobAssemblyLoadContext context)
        {
            if (context != null)
            {
                context.Unload();
                //for (int i = 0; context.weakReference.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

            }
        }
    }
}
