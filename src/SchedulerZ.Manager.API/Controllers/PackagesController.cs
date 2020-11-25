using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;
using SchedulerZ.Remoting;
using SchedulerZ.Route;

namespace SchedulerZ.Manager.API.Controllers
{
    public class PackagesController : BaseApiController
    {
        private readonly string _jobDirectory;
        private readonly string[] _allowedFileExtension;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PackagesController> _logger;

        private readonly ISchedulerRemoting _schedulerRemoting;
        private readonly IServiceRoute _serviceRoute;
        public PackagesController(ILogger<PackagesController> logger, IHostEnvironment hostEnvironment, ISchedulerRemoting schedulerRemoting, IServiceRoute serviceRoute)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _schedulerRemoting = schedulerRemoting;
            _serviceRoute = serviceRoute;

            _jobDirectory = Config.Options.JobDirectory;
            var allowedFileExtensions = Config.Options.JobAllowedFileExtension;
            _allowedFileExtension = string.IsNullOrWhiteSpace(allowedFileExtensions) ? new string[] { ".zip" } : allowedFileExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> UploadPackage()
        {
            var files = Request.Form.Files;

            var response = new List<UploadPackageResponse>();
            foreach (var file in files)
            {
                var result = new UploadPackageResponse();
                var uploadPath = "";
                try
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (!_allowedFileExtension.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                    {
                        result.Success = false;
                        result.ErrorMessage = "后缀不支持";
                        response.Add(result);
                        continue;
                    }

                    result.FileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}{DateTime.Now.FormatString("yyyyMMddHHmmss")}{fileExtension}";
                    var uploadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, _jobDirectory);
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    uploadPath = Path.Combine(uploadDirectory, result.FileName);
                    var webPath = string.Concat("/", _jobDirectory, "/", result.FileName);

                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        result.Success = true;
                        result.Url = webPath;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{file.FileName} {file.ContentType}");
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                }
                response.Add(result);

                if (result.Success)
                {
                    var services = await _serviceRoute.QueryServices();
                    foreach (var service in services)
                    {
                        if (service.Name == "worker")
                            await _schedulerRemoting.UploadFile(uploadPath, service);

                    }
                }
            }

            return BaseResponse<List<UploadPackageResponse>>.GetBaseResponse(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<BaseResponse> DownloadPackage(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, _jobDirectory, packageName).Replace('\\', Path.DirectorySeparatorChar);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", packageName);
        }
    }
}
