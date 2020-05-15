using Grpc.Core;
using Microsoft.Extensions.Hosting;
using SchedulerZ.gRPC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting.gRPC
{
    public class GrpcHostedService : IHostedService
    {
        private static Server _grpcServer;

        private readonly SchedulerService.SchedulerServiceBase _schedulerServiceBase;
        private readonly GrpcServiceConfig _config;
        public GrpcHostedService(SchedulerService.SchedulerServiceBase schedulerServiceBase, GrpcServiceConfig config)
        {
            _schedulerServiceBase = schedulerServiceBase;
            _config = config;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var channelOptions = new List<ChannelOption>()
                    {
                        new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                        new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue)
                    };

                _grpcServer = new Server(channelOptions)
                {
                    Services = { SchedulerService.BindService(_schedulerServiceBase) },
                    Ports = { new ServerPort(_config.Host, _config.Port, ServerCredentials.Insecure) }
                };

                _grpcServer.Start();
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _grpcServer?.ShutdownAsync();
        }
    }
}
