﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    public class LoadBalancerConfig
    {
        public string Type { get; set; } = "FirstLoadBalancer";

        public string Key { get; set; }

        public int ExpiryInMs { get; set; }
    }
}
