 using Grpc.Core;
using SchedulerZ.LoadBalancer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public interface IEndpointStrategy
    {
        ServerCallInvoker Get(string serviceName);
        void Revoke(string serviceName, ServerCallInvoker failedCallInvoker);
    }


    public class EndpointStrategy : IEndpointStrategy
    {
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, Channel> _channels = new ConcurrentDictionary<string, Channel>();

        private readonly ILoadBalancerFactory _loadBalancerFactory;
        public EndpointStrategy(ILoadBalancerFactory loadBalancerFactory)
        {
            _loadBalancerFactory = loadBalancerFactory;
        }


        public ServerCallInvoker Get(string serviceName)
        {
            var service = _loadBalancerFactory.Get().Lease(serviceName).GetAwaiter().GetResult();
            if (service == null)
                return null;

            var target = $"{service.Address}:{service.Port}";

            if (_channels.TryGetValue(target, out Channel channel))
                return new ServerCallInvoker(channel);

            lock (_lock)
            {
                if (!_channels.TryGetValue(target, out channel))
                {
                    var channelOptions = new List<ChannelOption>()
                    {
                        new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                        new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue)
                    };

                    channel = new Channel(target, ChannelCredentials.Insecure, channelOptions);
                    _channels.AddOrUpdate(target, channel, (key, value) => channel);

                }
                return new ServerCallInvoker(channel);
            }
        }

        public void Revoke(string serviceName, ServerCallInvoker failedCallInvoker)
        {
            lock (_lock)
            {
                if (failedCallInvoker == null)
                    return;

                var failedChannel = failedCallInvoker.Channel;
                if (!_channels.TryGetValue(serviceName, out Channel channel))
                    return;

                _channels.TryRemove(failedChannel.Target, out failedChannel);


                // add black TODO

                failedChannel.ShutdownAsync();

            }
        }
    }
}
