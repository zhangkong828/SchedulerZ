using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Runtime.Loader.AssemblyLoadContext;

namespace SchedulerZ.Domain
{
    public class DomainManager
    {
        public readonly static AssemblyDomain Default;
        public readonly static ConcurrentDictionary<string, WeakReference> DomainDic;

        public readonly static string CurrentPath;
        static DomainManager()
        {
            CurrentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicLib");
            if (!Directory.Exists(CurrentPath))
            {
                Directory.CreateDirectory(CurrentPath);
            }

            DomainDic = new ConcurrentDictionary<string, WeakReference>();
            Default = new AssemblyDomain("Default");

        }

        public static AssemblyDomain RandomCreate
        {
            get { return Create("N" + Guid.NewGuid().ToString("N")); }
        }

        public static AssemblyDomain Create(string key)
        {
            if (DomainDic.ContainsKey(key))
            {
                return (AssemblyDomain)(DomainDic[key].Target);
            }
            else
            {
                Clear();
                return new AssemblyDomain(key);
            }
        }

        public static void Clear()
        {
            foreach (var item in DomainDic)
            {
                if (!item.Value.IsAlive)
                {
                    DomainDic.TryRemove(item.Key, out _);
                }
            }
        }


        public static ContextualReflectionScope Lock(string key)
        {
            if (DomainDic.ContainsKey(key))
            {
                return ((AssemblyDomain)(DomainDic[key].Target)).EnterContextualReflection();
            }
            return Default.EnterContextualReflection();

        }

        public static ContextualReflectionScope Lock(AssemblyDomain domain)
        {
            return domain.EnterContextualReflection();
        }

        public static ContextualReflectionScope CreateAndLock(string key)
        {
            return Lock(Create(key));
        }


        public static AssemblyDomain CurrentDomain
        {
            get
            {
                return CurrentContextualReflectionContext == default ?
                    Default :
                    (AssemblyDomain)CurrentContextualReflectionContext;
            }
        }


        public static void Add(string key, AssemblyDomain domain)
        {
            if (DomainDic.ContainsKey(key))
            {
                if (!DomainDic[key].IsAlive)
                {
                    DomainDic[key] = new WeakReference(domain);
                }
            }
            else
            {
                DomainDic[key] = new WeakReference(domain, trackResurrection: true);
            }
        }

        public static WeakReference Remove(string key)
        {
            if (DomainDic.ContainsKey(key))
            {
                DomainDic.TryRemove(key, out var result);
                if (result != default)
                {
                    ((AssemblyDomain)(result.Target)).Dispose();
                }
                return result;
            }
            return null;

        }

        public static bool IsDeleted(string key)
        {
            if (DomainDic.ContainsKey(key))
            {
                return !DomainDic[key].IsAlive;
            }
            return true;
        }

        public static AssemblyDomain Get(string key)
        {
            if (DomainDic.ContainsKey(key))
            {
                return (AssemblyDomain)DomainDic[key].Target;
            }
            return null;
        }

        public static int Count(string key)
        {
            return ((AssemblyDomain)(DomainDic[key].Target)).Count;
        }

    }
}
