using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class DbConnector
    {
        public DbProvider Provider { get; set; }

        public string ConnectionString { get; set; }
    }

    public enum DbProvider
    {
        MySQL,
        SQLServer,
        PostgreSQL
    }
}
