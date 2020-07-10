using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using EasyCaching.Core;
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

namespace SchedulerZ.Manager.API.Controllers
{
    public class PackagesController : BaseApiController
    {
        private readonly string[] _allowedFileExtension = { ".zip", ".7z", ".rar" };
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PackagesController> _logger;

        private readonly CSRedisClient _redisClient;
        private readonly IEasyCachingProvider _cachingProvider;
        public PackagesController(ILogger<PackagesController> logger, IHostEnvironment hostEnvironment, CSRedisClient redisClient)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _redisClient = redisClient;
        }

        [HttpPost]
        public ActionResult<BaseResponse> UploadPackage()
        {
            var files = Request.Form.Files;

            var response = new List<UploadPackageResponse>();
            foreach (var file in files)
            {
                var result = new UploadPackageResponse();
                try
                {
                    result.FileName = file.FileName;
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (!_allowedFileExtension.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                    {
                        result.Success = false;
                        result.ErrorMessage = "后缀不支持";
                        response.Add(result);
                        continue;
                    }

                    var uploadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Packages");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    var uploadPath = Path.Combine(uploadDirectory, file.FileName);
                    var webPath = Path.Combine("/Packages", file.FileName).Replace("\\", "/");

                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
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
            }

            return BaseResponse<List<UploadPackageResponse>>.GetBaseResponse(response);
        }


    }
}
