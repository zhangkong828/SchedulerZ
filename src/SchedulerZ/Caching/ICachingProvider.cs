using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Caching
{
    public interface ICachingProvider
    {
        ICaching CreateCaching();
    }
}
