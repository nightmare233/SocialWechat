using Social.WebApi.Controllers;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Social.WebApi.Core
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            operation.parameters = operation.parameters ?? new List<Parameter>();

            //if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType != typeof(AccountsController))
            //{
            operation.parameters.Add(new Parameter
            {
                name = "siteId",
                required = true,
                type = "integer",
                @in = "query",
                @default = "10000"
            });
            //}
        }
    }
}