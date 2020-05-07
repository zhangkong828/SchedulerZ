using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul.ClientProvider
{
    public class ClientEntry
    {
        public ClientEntry(Uri address, bool health = false)
        {
            Address = address;
            Host = address.Host;
            Port = address.Port;
            Health = health;
        }
        public Uri Address { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Health { get; set; }
        public int UnhealthyTimes { get; set; }
    }
}
