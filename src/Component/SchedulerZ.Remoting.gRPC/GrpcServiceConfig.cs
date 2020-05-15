using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC
{
    public class GrpcServiceConfig
    {
        public string Host { get; set; } = "0.0.0.0";
        public int Port { get; set; } = 10001;
    }
}
