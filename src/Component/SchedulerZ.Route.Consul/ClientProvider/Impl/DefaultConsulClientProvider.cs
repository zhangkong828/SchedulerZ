﻿using Consul;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Route.Consul.ClientProvider.Impl
{
    public class DefaultConsulClientProvider : IConsulClientProvider, IDisposable
    {
        private readonly ConsulServiceRouteConfig _config;
        private readonly int _timeout;
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, ClientEntry> _consulClients = new ConcurrentDictionary<string, ClientEntry>();
        public DefaultConsulClientProvider(ConsulServiceRouteConfig config)
        {
            _config = config;

            _timeout = _config.NodeCheckTimeOut;

            Initialize();

            var timeSpan = _config.NodeCheckInterval;
            _timer = new Timer(async s =>
            {
                await NodeCheckHealth();
            }, null, timeSpan, timeSpan);

        }

        private void Initialize()
        {
            var client = new ConsulClient(config =>
            {
                var uriBuilder = new UriBuilder(_config.Scheme, _config.Host, _config.Port);
                config.Address = uriBuilder.Uri;
            });

            var nodes = client.Catalog.Nodes().GetAwaiter().GetResult();
            if (nodes.Response.Length > 0)
            {
                foreach (var node in nodes.Response)
                {
                    var address = new UriBuilder(_config.Scheme, node.Address, _config.Port).Uri;
                    if (IsHealth(address.Host, _config.Port))
                        _consulClients.TryAdd(node.Name, new ClientEntry(address, true));
                }
            }
        }

        private Task NodeCheckHealth()
        {
            foreach (var item in _consulClients)
            {
                if (IsHealth(item.Value.Host, item.Value.Port))
                {
                    item.Value.Health = true;
                    item.Value.UnhealthyTimes = 0;
                }
                else
                {
                    item.Value.Health = false;
                    item.Value.UnhealthyTimes++;
                }
            }
            return Task.FromResult(true);
        }


        public ConsulClient GetClient()
        {
            ConsulClient client = null;
            var node = _consulClients.Values.FirstOrDefault(x => x.Health);
            if (node != null)
            {
                client = new ConsulClient(config =>
                {
                    config.Address = node.Address;
                });
            }
            return client;
        }

        public bool IsHealth(string host, int port)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    IAsyncResult connResult = socket.BeginConnect(host, port, null, null);

                    connResult.AsyncWaitHandle.WaitOne(_timeout, true);

                    if (!connResult.IsCompleted)
                    {
                        socket.Close();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
        }
    }
}
