using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Core.Grpc
{
    public class EndpointStrategy : IEndpointStrategy
    {
        public ServerCallInvoker Get(string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Revoke(string serviceName, ServerCallInvoker failedCallInvoker)
        {
            throw new NotImplementedException();
        }
    }
}
