using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSRedis;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchedulerZ.Logging.Log4Net;
using SchedulerZ.Manager.API.Filter;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Route.Consul;
using SchedulerZ.Store.MySQL;

namespace SchedulerZ.Manager.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.UseMySQL(options => options.ConnectionString = Configuration.GetConnectionString("DefaultConnection"));

            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));

            services.AddEasyCaching(options =>
            {
                //use memory cache that named default
                options.UseInMemory("default");
            });

            services.AddSingleton(_ => new CSRedisClient(Configuration.GetValue<string>("Redis:Connection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = TokenValidatedFilter.OnTokenValidated
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration.GetValue<string>("JWTConfig:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("JWTConfig:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JWTConfig:IssuerSigningKey")))
                };
            });

            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var error = context.ModelState.GetValidationSummary();
                    return new JsonResult(BaseResponse<List<ModelState>>.GetBaseResponse(ResponseStatusType.ParameterError, error));
                };
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.UseLog4Net();

            //Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SchedulerZ.Manager API",
                    Description = "A Simple SchedulerZ.Manager Web API"
                });

                //options.OperationFilter<SwaggerOperationFilter>();

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                options.IncludeXmlComments(Path.Combine(basePath, "SchedulerZ.Manager.API.xml"));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });

            //Origin
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader();
                    });
            });

            //services.UseConsulServiceRoute(config =>
            //{
            //    config.Host = Configuration.GetValue<string>("ConsulConfig:Host");
            //    config.Port = Configuration.GetValue<int>("ConsulConfig:Port");

            //}, registerService =>
            //{
            //    registerService.Name = "manager";
            //    registerService.Address = "192.168.31.200";
            //    registerService.Port = 10001;
            //    registerService.HealthCheckType = "HTTP";
            //    registerService.HealthCheck = "http://192.168.31.200:10001/api/health/check";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchedulerZ.Manager API");
            });

            //app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
