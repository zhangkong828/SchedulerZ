using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting.gRPC
{
    public class FileHostedService : IHostedService
    {
        private const int _taskInterval = 10;
        private const int _fileClearExpirateDay = 5;
        private bool _fileClearTaskStatus;
        public FileHostedService()
        {
            _fileClearTaskStatus = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var baseDirectory = $"{AppContext.BaseDirectory}\\{Config.Options.JobDirectory}".Replace('\\', Path.DirectorySeparatorChar);
            new Task(() =>
            {
                while (_fileClearTaskStatus)
                {
                    try
                    {
                        var files = Directory.GetFiles(baseDirectory, "*.zip");
                        var dateTimeNow = DateTime.Now;
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            if (fileName.Length > 14)
                            {
                                var dateTimeString = fileName.Substring(fileName.Length - 14);
                                if (DateTime.TryParseExact(dateTimeString, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                                {
                                    if ((dateTimeNow - dateTime).Days >= _fileClearExpirateDay)
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        Task.Delay(TimeSpan.FromMinutes(_taskInterval));
                    }
                }

            }, cancellationToken, TaskCreationOptions.LongRunning).Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _fileClearTaskStatus = false;
            return Task.CompletedTask;
        }
    }
}
