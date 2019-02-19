using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlogDotNet.Infrastructure.Middlewares
{
    public class AppExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<AppExceptionMiddleware> _logger;
        private readonly IStringLocalizer<AppExceptionMiddleware> _localizer;

        public AppExceptionMiddleware(
            RequestDelegate next,
            IStringLocalizer<AppExceptionMiddleware> localizer,
            ILogger<AppExceptionMiddleware> logger)
        {
            this.next = next;
            this._logger = logger;
            this._localizer = localizer;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger, _localizer);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger<AppExceptionMiddleware> logger,
            IStringLocalizer<AppExceptionMiddleware> localizer)
        {
            object errors = null;

            switch (exception)
            {
                case RestException re:
                    errors = re.Errors;
                    context.Response.StatusCode = (int) re.Code;
                    break;
                case Exception e:
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            var result = JsonConvert.SerializeObject(new
            {
                errors
            });

            await context.Response.WriteAsync(result);
        }
    }

    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }

        public object Errors { get; set; }

        public HttpStatusCode Code { get; }
    }
}