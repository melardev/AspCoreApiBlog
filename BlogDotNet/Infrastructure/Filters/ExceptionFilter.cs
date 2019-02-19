using BlogDotNet.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BlogDotNet.Infrastructure.Filters
{
    public class AppExceptionFilter : ExceptionFilterAttribute
    {
        private IHostingEnvironment _hostingEnvironment;

        public AppExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            // if (_hostingEnvironment.IsDevelopment()) { }
            //base.OnException(context);
            if (context.Exception is PermissionDeniedException)
            {
            }
            else if (context.Exception is ResourceNotFoundException)
            {
                context.Result = new ViewResult()
                {
                    ViewName = "ErrorPage",
                    ViewData = new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        Model = @"Resource not found"
                    }
                };
            }
        }
    }
}