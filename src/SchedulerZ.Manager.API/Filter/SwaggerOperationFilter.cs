using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Filter
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters ?? new List<OpenApiParameter>();
            try
            {
                var info = context.MethodInfo;
                if (context.ApiDescription.TryGetMethodInfo(out info))
                {
                    Attribute allowAnonymous = info.GetCustomAttribute(typeof(AllowAnonymousAttribute));
                    if (allowAnonymous == null)
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Description = "示例: Bearer {Token}",
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" }

                        });
                    }
                }

            }
            catch { }
        }
    }
}
