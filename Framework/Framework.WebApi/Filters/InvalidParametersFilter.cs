using Framework.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Framework.WebApi.Filters
{
    public class InvalidParametersFilter : ActionFilterAttribute
    {
        // Cache used to store the required parameters for each request based on the
        // request's http method and local path.
        private readonly ConcurrentDictionary<Tuple<HttpMethod, string>, List<string>> _Cache =
            new ConcurrentDictionary<Tuple<HttpMethod, string>, List<string>>();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Get the request's required parameters.
            List<string> requiredParameters = this.GetRequiredParameters(actionContext);

            Dictionary<string, object[]> maxLengthDic = this.GetParametersAndValues(actionContext, "MaxLengthAttribute");
            Dictionary<string, object[]> rangeDic = this.GetParametersAndValues(actionContext, "RangeAttribute");
            var invliadParamterKeys = this.ValidateParameters(actionContext, requiredParameters, maxLengthDic, rangeDic);

            if (!invliadParamterKeys.Any())
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                throw new ExceptionWithCode(40000, $"Parameter '{invliadParamterKeys.FirstOrDefault()}' is required or invalid.");
            }
        }

        private IList<string> ValidateParameters(HttpActionContext actionContext, List<string> requiredParameters, Dictionary<string, object[]> maxLengthDic, Dictionary<string, object[]> rangeDic)
        {
            List<string> invalidParameterKeys = new List<string>();
            // If the list of required parameters is null or containst no parameters 
            // then there is nothing to validate.  
            // Return true.
            if ((requiredParameters == null || requiredParameters.Count == 0) && (maxLengthDic == null || maxLengthDic.Count == 0) && (rangeDic == null || rangeDic.Count == 0))
            {
                return new List<string>();
            }

            if (!(requiredParameters == null || requiredParameters.Count == 0))
            {
                // Attempt to find at least one required parameter that is null.
                invalidParameterKeys =
                    actionContext
                    .ActionArguments
                    .Where(a => requiredParameters.Contains(a.Key) && (a.Value == null || isEmpty(a.Value))).Select(a => a.Key).ToList();

                // If a null required paramter was found then return false.  
                // Otherwise, return true.
            }

            if (!(maxLengthDic == null || maxLengthDic.Count == 0))
            {
                foreach (var dic in maxLengthDic)
                {
                    invalidParameterKeys.AddRange(
                        actionContext
                        .ActionArguments
                        .Where(a => a.Key == dic.Key && (a.Value.ToString().Length > (int)(dic.Value[0]))).Select(a => a.Key).ToList());
                }
            }

            if (!(rangeDic == null || rangeDic.Count == 0))
            {
                foreach (var dic in rangeDic)
                {
                    invalidParameterKeys.AddRange(
                        actionContext
                        .ActionArguments
                        .Where(a => a.Key == dic.Key && (((int)(a.Value) < (int)(dic.Value[0])) || ((int)(a.Value) > (int)(dic.Value[1])))).Select(a => a.Key).ToList());
                }
            }
            return invalidParameterKeys;
        }

        private bool isEmpty(object value)
        {
            if (value.GetType() == typeof(String))
            {
                return string.IsNullOrWhiteSpace(value.ToString());
            }
            return false;
        }

        private List<string> GetRequiredParameters(HttpActionContext actionContext)
        {
            // Instantiate a list of strings to store the required parameters.
            List<string> result = null;

            // Instantiate a tuple using the request's http method and the local path.
            // This will be used to add/lookup the required parameters in the cache.
            Tuple<HttpMethod, string> request =
                new Tuple<HttpMethod, string>(
                    actionContext.Request.Method,
                    actionContext.Request.RequestUri.LocalPath);

            // Attempt to find the required parameters in the cache.
            if (!this._Cache.TryGetValue(request, out result))
            {
                // If the required parameters were not found in the cache then get all
                // parameters decorated with the 'RequiredAttribute' from the action context.
                result =
                    actionContext
                    .ActionDescriptor
                    .GetParameters()
                    .Where(p => p.GetCustomAttributes<RequiredAttribute>().Any())
                    .Select(p => p.ParameterName)
                    .ToList();

                // Add the required parameters to the cache.
                this._Cache.TryAdd(request, result);
            }

            // Return the required parameters.
            return result;
        }

        private Dictionary<string, object[]> GetParametersAndValues(HttpActionContext actionContext, string AttributeType)
        {
            Dictionary<string, object[]> dic = new Dictionary<string, object[]>();

            {
                if (AttributeType == "MaxLengthAttribute")
                {
                    var Parameters = actionContext
                    .ActionDescriptor
                    .GetParameters()
                    .Where(p => p.GetCustomAttributes<MaxLengthAttribute>().Any());
                    foreach (var Parameter in Parameters)
                    {
                        var b = Parameter.GetCustomAttributes<MaxLengthAttribute>().FirstOrDefault();
                        if (b != null)
                        {
                            dic.Add(Parameter.ParameterName, new object[] { ((MaxLengthAttribute)b).Length });
                        }
                    }
                }
                else if (AttributeType == "RangeAttribute")
                {
                    var Parameters = actionContext
                    .ActionDescriptor
                    .GetParameters()
                    .Where(p => p.GetCustomAttributes<RangeAttribute>().Any());
                    foreach (var Parameter in Parameters)
                    {
                        var b = Parameter.GetCustomAttributes<RangeAttribute>().FirstOrDefault();
                        if (b != null)
                        {
                            dic.Add(Parameter.ParameterName, new object[] { ((RangeAttribute)b).Minimum, ((RangeAttribute)b).Maximum });
                        }
                    }
                }
            }

            return dic;
        }
    }
}
