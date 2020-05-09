using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Core.Grpc
{
    public interface IEndpointStrategy
    {
        ServerCallInvoker Get(string serviceName);
        void Revoke(string serviceName, ServerCallInvoker failedCallInvoker);
    }
}
