using ClinicalTrialAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ClinicalTrialAPI.eDelegation
{
    public class ValidationActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ApiResponse _ApiResponse = new ApiResponse();
            var modelState = actionContext.ModelState;

            if (!modelState.IsValid)
            {
                _ApiResponse.success = 0;
                _ApiResponse.message = "Invalid request";
                var errors = new List<string>();
                foreach (var state in modelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        if(error.ErrorMessage!="")
                         errors.Add(error.ErrorMessage);
                        else
                            errors.Add(error.Exception.Message);
                    }
                }
                _ApiResponse.data = errors;

                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.BadRequest, _ApiResponse, JsonMediaTypeFormatter.DefaultMediaType);
            }
        }
    }
}