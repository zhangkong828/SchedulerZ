using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Response
{
    public class UploadPackageResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string FileName { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
    }
}
