using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC
{
    public interface IEndpointStrategy
    {
        ServerCallInvoker Get(string serviceName);
        void Revoke(string serviceName, ServerCallInvoker failedCallInvoker);
    }
}
