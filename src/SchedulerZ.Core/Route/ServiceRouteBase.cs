using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Core.Route
{
    public abstract class ServiceRouteBase : IServiceRoute
    {

        private EventHandler<ServiceRouteEventArgs> _created;
        private EventHandler<ServiceRouteEventArgs> _removed;
        private EventHandler<ServiceRouteChangedEventArgs> _changed;

        public abstract Task<IEnumerable<ServiceRouteDescriptor>> GetServices(string key);

        public abstract Task<bool> RegisterService(ServiceRouteDescriptor service);
        public abstract Task<bool> DeregisterService(ServiceRouteDescriptor service);


        /// <summary>
        /// 服务被创建
        /// </summary>
        public event EventHandler<ServiceRouteEventArgs> Created
        {
            add { _created += value; }
            remove { _created -= value; }
        }

        /// <summary>
        /// 服务被删除
        /// </summary>
        public event EventHandler<ServiceRouteEventArgs> Removed
        {
            add { _removed += value; }
            remove { _removed -= value; }
        }

        /// <summary>
        /// 服务被修改
        /// </summary>
        public event EventHandler<ServiceRouteChangedEventArgs> Changed
        {
            add { _changed += value; }
            remove { _changed -= value; }
        }

        protected void OnCreated(params ServiceRouteEventArgs[] args)
        {
            if (_created == null)
                return;

            foreach (var arg in args)
                _created(this, arg);
        }

        protected void OnChanged(params ServiceRouteChangedEventArgs[] args)
        {
            if (_changed == null)
                return;

            foreach (var arg in args)
                _changed(this, arg);
        }

        protected void OnRemoved(params ServiceRouteEventArgs[] args)
        {
            if (_removed == null)
                return;

            foreach (var arg in args)
                _removed(this, arg);
        }

    }
}
