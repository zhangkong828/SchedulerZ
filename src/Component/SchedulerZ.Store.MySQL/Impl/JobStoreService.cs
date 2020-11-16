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
    }
}
