using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace SchedulerZ.Core.Scheduler
{
    public class JobAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;
        public JobAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        }

        /// <summary>
        /// 加载托管程序集
        /// </summary>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                //return LoadFromAssemblyPath(assemblyPath);
                using (var stream = new System.IO.FileStream(assemblyPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    return LoadFromStream(stream);
                }
            }

            return null;
        }

        /// <summary>
        /// 加载非托管程序集
        /// </summary>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
