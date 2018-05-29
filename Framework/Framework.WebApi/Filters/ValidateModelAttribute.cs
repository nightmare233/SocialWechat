using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Framework.WebApi
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var validationResult = GetValidationResults(actionContext);
                var errorMessage = validationResult.Select(t => t.ErrorMessage).FirstOrDefault();

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorInfo(40000, errorMessage)
                    );
            }
        }

        private List<ValidationResult> GetValidationResults(HttpActionContext actionContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            foreach (var state in actionContext.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    string errorInfo = string.IsNullOrWhiteSpace(error.ErrorMessage) ? string.Empty : error.ErrorMessage;
                    if (string.IsNullOrWhiteSpace(error.ErrorMessage) && error.Exception != null)
                    {
                        errorInfo = "Invalid data format";
                    }

                    if (!string.IsNullOrWhiteSpace(errorInfo))
                    {
                        result.Add(new ValidationResult(errorInfo, new[] { state.Key }));
                    }
                }
            }

            return result;
        }
    }
}
