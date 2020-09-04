using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class PageData<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public long TotalCount { get; set; }

        public long TotalPage
        {
            get
            {
                try
                {
                    return (long)Math.Ceiling(TotalCount / (double)PageSize);
                }
                catch
                {
                    return 0;
                }

            }
        }

        public List<T> List { get; set; }
    }
}
