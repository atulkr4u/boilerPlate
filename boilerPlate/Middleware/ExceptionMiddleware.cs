using System;
using System.Net;
using System.Text;
using boilerPlate.Domain.DomainBase;
using boilerPlate.Domain.Helpers;

namespace boilerPlate.Middleware
{

    public class ExceptionMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                httpContext.Request.EnableBuffering();
                await _next(httpContext);

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception.GetType() == typeof(ApplicationException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                return context.Response.WriteAsync(new BaseResponse()
                {
                    ExceptionMessage = exception.Message,
                }.SerializeToJson());
            }
            else
            {
                _logger.LogCritical(exception, "OtherException");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(new BaseResponse()
                {
                    ExceptionMessage = "System Error",
                }.SerializeToJson());
            }
        }
        private string GetRequest(HttpContext httpContext)
        {
            string result = string.Empty;
            try
            {
                httpContext.Request.Body.Position = 0;
                using (StreamReader reader
                          = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    result = reader.ReadToEnd();
                }
                httpContext.Request.Body.Position = 0;
            }
            catch
            {

            }
            return result;
        }
    }
}

