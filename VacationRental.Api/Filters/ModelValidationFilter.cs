using System;
using System.Linq;
using Logistics.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VacationRental.Api.Filters
{
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var error = new ErrorViewModel
                {
                    ErrorDescription = CreateErrorDescription(context.ModelState)
                };
                context.Result = new BadRequestObjectResult(error);
            }

            base.OnActionExecuting(context);
        }

        private static string CreateErrorDescription(ModelStateDictionary modelState)
        {
            var allErrors = modelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage));
            return string.Join(Environment.NewLine, allErrors);
        }
    }
}