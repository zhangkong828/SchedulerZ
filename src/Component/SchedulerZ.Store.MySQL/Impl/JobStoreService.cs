using Microsoft.EntityFrameworkCore;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SchedulerZ.Store.MySQL.Impl
{
    public class JobStoreService : IJobStore
    {
        private readonly SchedulerZContext _context;
        public JobStoreService(SchedulerZContext context)
        {
            _context = context;
        }

        public JobEntity QueryJob(string id)
        {
            return _context.Jobs.AsNoTracking().Where(x => x.Status != -1).SingleOrDefault(x => x.Id == id);
        }

        public List<JobEntity> QueryJobList<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<JobEntity, bool>>> wheres, Expression<Func<JobEntity, TOrderKey>> orderBy, bool isAsc, out int total)
        {
            var query = _context.Jobs.AsNoTracking().Where(x => x.Status != -1);
            if (wheres.Any())
            {
                foreach (var item in wheres)
                {
                    query = query.Where(item);
                }
            }
            total = query.Count();
            query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool AddJob(JobEntity entity)
        {
            _context.Jobs.Add(entity);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteJob(string id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return false;

            job.Status = -1;
            _context.Jobs.Update(job);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateJob(JobEntity entity)
        {
            _context.Jobs.Update(entity);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateRunJob(string id, DateTimeOffset lastRunTime, DateTimeOffset? nextRunTime)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return false;

            job.LastRunTime = lastRunTime.LocalDateTime;
            if (nextRunTime.HasValue)
                job.NextRunTime = nextRunTime.GetValueOrDefault().LocalDateTime;
            else
            {
                job.NextRunTime = null;
                job.Status = (int)JobStatus.Stop;
            }
            job.TotalRunCount += 1;

            _context.Set<JobEntity>().Attach(job);
            _context.Entry(job).Property(x => x.LastRunTime).IsModified = true;
            _context.Entry(job).Property(x => x.NextRunTime).IsModified = true;
            _context.Entry(job).Property(x => x.Status).IsModified = true;
            _context.Entry(job).Property(x => x.TotalRunCount).IsModified = true;
            return _context.SaveChanges() > 0;
        }

        public bool UpdateEntity<T>(T entity, Expression<Func<T, object>>[] updatedProperties)
            where T : class
        {
            _context.Set<T>().Attach(entity);
            if (updatedProperties.Any())
            {
                foreach (var property in updatedProperties)
                {
                    _context.Entry<T>(entity).Property(property).IsModified = true;
                }
            }

            return _context.SaveChanges() > 0;
        }

        public List<JobEntity> QueryRunningJob(string nodeHost, int nodePort)
        {
            return _context.Jobs.AsNoTracking().Where(x => (x.Status == (int)JobStatus.Running || x.Status == (int)JobStatus.Paused) && x.NodeHost == nodeHost && x.NodePort == nodePort).ToList();
        }
    }
}
