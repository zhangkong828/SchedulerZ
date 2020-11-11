using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchedulerZ.Store
{
    public interface IJobStore
    {
        List<JobEntity> QueryJobList<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<JobEntity, bool>>> wheres, Expression<Func<JobEntity, TOrderKey>> orderBy, bool isAsc, out int total);
        JobEntity QueryJob(string id);
        bool AddJob(JobEntity entity);
        bool UpdateJob(JobEntity entity);
        bool DeleteJob(string id);
    }
}
