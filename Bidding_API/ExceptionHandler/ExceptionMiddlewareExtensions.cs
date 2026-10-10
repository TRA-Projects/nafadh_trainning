using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bidding_API.ExceptionHandler
{
    public static class  ExceptionMiddlewareExtensions
    {

        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger,IConfiguration configuration )
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    string exception = contextFeature.Error.Message;

                    Guid errorId = Guid.NewGuid();

                    string formattedException = string.Format("{0} - {1} : {2}", errorId.ToString(), exception.GetType().Name, exception.ToString());

                    logger.LogError(formattedException, (Exception)contextFeature.Error);
                    string errormsg = String.Format(configuration["GlobalErrorMessage"], errorId);

                    if (contextFeature != null)
                    {    
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = errormsg
                        }.ToString());
                    }
                });
            });
        
    }
}
}
