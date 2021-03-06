﻿using Grpc.Core;
using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public interface IGrpcClientFactory<T> where T : ClientBase
    {
        T Get(ServiceRouteDescriptor service);
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


        public T Get(ServiceRouteDescriptor service)
        {
            var callInvoker = new ClientCallInvoker(service, _endpointStrategy, _config.MaxRetry);
            var client = (T)Activator.CreateInstance(typeof(T), callInvoker);
            return client;
        }
    }
}
