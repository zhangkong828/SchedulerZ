using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Core.Grpc
{
    public class EndpointStrategy : IEndpointStrategy
    {
        private readonly object _lock = new object();
        //private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, List<ServerCallInvoker>> _invokers = new ConcurrentDictionary<string, List<ServerCallInvoker>>();
        private readonly ConcurrentDictionary<string, Channel> _channels = new ConcurrentDictionary<string, Channel>();

        public EndpointStrategy()
        {

        }


        public ServerCallInvoker Get(string serviceName)
        {
            if (_invokers.TryGetValue(serviceName, out List<ServerCallInvoker> callInvokers) &&
                callInvokers?.Count > 0)
                return ServicePollingPlicy.Random(callInvokers);

            lock (_lock)
            {
                if (_invokers.TryGetValue(serviceName, out callInvokers) &&
                    callInvokers?.Count > 0)
                    return ServicePollingPlicy.Random(callInvokers);

                callInvokers = SetCallInvokers(serviceName);
                if ((callInvokers?.Count ?? 0) <= 0 && ServiceBlackPlicy.Exist(serviceName))
                    callInvokers = SetCallInvokers(serviceName, false);

                return ServicePollingPlicy.Random(callInvokers);
            }
        }

        public void Revoke(string serviceName, ServerCallInvoker failedCallInvoker)
        {
            throw new NotImplementedException();
        }
    }
}
