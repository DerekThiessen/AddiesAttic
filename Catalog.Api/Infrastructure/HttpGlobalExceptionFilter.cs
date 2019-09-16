namespace Catalog.Api.Infrastructure
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System.Net;

    public class HttpGlobalExceptionFilter
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment hostingEnvironment, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public void OnException(ExceptionContext exceptionContext)
        {
            _logger.LogError(new EventId(exceptionContext.Exception.HResult),
                exceptionContext.Exception,
                exceptionContext.Exception.Message);

            var json = new JsonErrorResponse
            {
                Messages = new[] { "An error occurred." }
            };

            if (_hostingEnvironment.IsDevelopment())
                json.DeveloperMessage = exceptionContext.Exception;

            //exceptionContext.Result = new InternalServerErrorObjectResult(json);
            exceptionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            exceptionContext.ExceptionHandled = true;
        }


        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }
}
