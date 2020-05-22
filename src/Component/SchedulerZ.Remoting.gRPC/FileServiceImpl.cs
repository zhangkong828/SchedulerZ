using Grpc.Core;
using SchedulerZ.gRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting.gRPC
{
    public class FileServiceImpl : SchedulerZ.gRPC.FileService.FileServiceBase
    {
        public FileServiceImpl()
        {

        }

        public override Task<FileResponse> JobExists(Job request, ServerCallContext context)
        {
            return base.JobExists(request, context);
        }

        public override Task<FileResponse> UploadFile(IAsyncStreamReader<UploadRequest> requestStream, ServerCallContext context)
        {
            try
            {
                var response = new FileResponse();
                var fileExtMetadata = context.RequestHeaders.FirstOrDefault(a => a.Key == "fileExt");
                if (fileExtMetadata == null)
                {
                    response.Message = "未获取到文件后缀";
                    return Task.FromResult(response);
                }

                string fileExt = fileExtMetadata.Value;
            }
            catch (Exception)
            {

                throw;
            }
            return base.UploadFile(requestStream, context);
        }
    }
}
