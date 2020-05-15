using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public class GrpcClientConfig
    {
        public int MaxRetry { get; set; } = 0;
    }
}
