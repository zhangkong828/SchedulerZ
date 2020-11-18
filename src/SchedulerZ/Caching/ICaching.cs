﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Caching
{
    public interface ICaching
    {
        string Get(string key);
        T Get<T>(string key);
        bool Set(string key, object data, TimeSpan? expiry = null);
        bool Exists(string key);
        bool Remove(string key);
    }
}
