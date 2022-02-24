using System;
using System.Net;
using System.Threading.Tasks;
using DatingApp.API.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DatingApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly IHostingEnvironment env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostingEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.next = next;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,ex.Message);
                context.Response.StatusCode=(int) HttpStatusCode.InternalServerError;
                context.Response.ContentType="application/json";

                var response=env.IsDevelopment()?
                    new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString()):
                    new ApiException(context.Response.StatusCode,"Internal Server Error");

                var json=JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
                
            }
        }
    }
}