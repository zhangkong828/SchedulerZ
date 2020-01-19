using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SchedulerZ.Core.Scheduler
{
    public class JobFactory
    {
        public static JobBase CreateJobInstance(JobAssemblyLoadContext context, Guid sid, string assemblyName, string className)
        {
            try
            {
                string pluginLocation = GetJobAssemblyPath(sid, assemblyName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
                Type type = assembly.GetType(className, true, true);
                return Activator.CreateInstance(type) as JobBase;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetJobAssemblyPath(Guid sid, string assemblyName)
        {
            return $"{Directory.GetCurrentDirectory()}\\wwwroot\\plugins\\{sid}\\{assemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
