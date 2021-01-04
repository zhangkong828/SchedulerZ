using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Job.FundTest
{
    public class TaskHelper
    {
        /// <summary>
        /// 异常重试操作
        /// <code>
        /// await RetryOnFault(() => DownloadStringAsync(url), 3, () => Task.Delay(1000));
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <param name="maxTries"></param>
        /// <param name="retryWhen"></param>
        /// <returns></returns>
        public static async Task<T> RetryOnFault<T>(Func<Task<T>> function, int maxTries, Func<Task> retryWhen)
        {
            for (int i = 0; i < maxTries; i++)
            {
                try { return await function().ConfigureAwait(false); }
                catch { if (i == maxTries - 1) break; }
                await retryWhen().ConfigureAwait(false);
            }
            return default(T);
        }
    }
}
