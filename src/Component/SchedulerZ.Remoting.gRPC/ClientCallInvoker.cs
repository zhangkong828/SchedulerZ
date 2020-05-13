using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private readonly int _maxRetry;
        private readonly IEndpointStrategy _strategy;
        public ClientCallInvoker(IEndpointStrategy endpointStrategy, int maxRetry = 0)
        {
            _strategy = endpointStrategy;
            _maxRetry = maxRetry;
        }

        private TResponse Call<TResponse>(string serviceName, Func<CallInvoker, TResponse> call, int retryLeft)
        {
            while (true)
            {
                var callInvoker = default(ServerCallInvoker);
                try
                {
                    callInvoker = _strategy.Get(serviceName);
                    if (callInvoker == null)
                    {
                        throw new ArgumentNullException($"{serviceName}无可用节点");
                    }

                    var channel = callInvoker.Channel;
                    if (channel == null || channel.State == ChannelState.TransientFailure)
                    {
                        throw new RpcException(new Status(StatusCode.Unavailable, $"Channel Failure"));
                    }

                    //var response = default(TResponse);
                    //if (_tracer != null)
                    //    response = call(callInvoker.ClientIntercept(_tracer));
                    //else
                    //    response = call(callInvoker);
                    return call(callInvoker);
                }
                catch (RpcException ex)
                {
                    if (ex.Status.StatusCode == StatusCode.Unavailable)
                        _strategy.Revoke(serviceName, callInvoker);

                    if (0 > --retryLeft)
                    {
                        throw;// new Exception($"status: {ex.StatusCode.ToString()}, node: {callInvoker?.Channel?.Target}, message: {ex.Message}", ex);
                    }
                }
            }
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(method.ServiceName, ci => ci.AsyncClientStreamingCall(method, host, options), _maxRetry);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(method.ServiceName, ci => ci.AsyncDuplexStreamingCall(method, host, options), _maxRetry);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(method.ServiceName, ci => ci.AsyncServerStreamingCall(method, host, options, request), _maxRetry);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(method.ServiceName, ci => ci.AsyncUnaryCall(method, host, options, request), _maxRetry);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(method.ServiceName, ci => ci.BlockingUnaryCall(method, host, options, request), _maxRetry);
        }
    }
}
