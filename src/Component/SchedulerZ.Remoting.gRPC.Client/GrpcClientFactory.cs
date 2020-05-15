using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public interface IGrpcClientFactory<T> where T : ClientBase
    {
        T Get();
    }

    public class GrpcClientFactory<T> : IGrpcClientFactory<T> where T : ClientBase
    {
        private readonly IEndpointStrategy _endpointStrategy;
        private readonly GrpcClientConfig _config;
        public GrpcClientFactory(IEndpointStrategy endpointStrategy, GrpcClientConfig config)
        {
            _endpointStrategy = endpointStrategy;
            _config = config;
        }


        public T Get()
        {
            var callInvoker = new ClientCallInvoker(_endpointStrategy, _config.MaxRetry);
            var client = (T)Activator.CreateInstance(typeof(T), callInvoker);
            return client;
        }
    }
}
