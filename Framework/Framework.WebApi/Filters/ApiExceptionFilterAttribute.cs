using Framework.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace Framework.WebApi.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ApiExceptionFilterAttribute));

        public override void OnException(HttpActionExecutedContext context)
        {
            logger.Error(context.Exception.Message, context.Exception);

            var httpException = context.Exception as HttpException;
            if (httpException != null)
            {
                context.Response = context.Request.CreateResponse(
                    (HttpStatusCode)httpException.GetHttpCode(),
                    new ErrorInfo(context.Exception.Message)
                    );

                return;
            }

            var codeException = context.Exception as ExceptionWithCode;
            if (codeException != null)
            {
                context.Response = context.Request.CreateResponse(
                codeException.HttpStatusCode,
                new ErrorInfo(codeException.ErrorCode, codeException.Message)
                );

                return;
            }

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new ErrorInfo(context.Exception.Message)
            );

        }
    }
}
