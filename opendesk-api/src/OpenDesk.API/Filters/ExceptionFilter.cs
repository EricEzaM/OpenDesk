using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenDesk.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Filters
{
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			context.HttpContext.Response.StatusCode = 500;

			var errors = new List<string>();
			errors.Add(context.Exception.Message);

			var res = new ApiResponse()
			{
				Outcome = OperationOutcome.Error($"An error was encountered while processing the request '{context.ActionDescriptor.AttributeRouteInfo.Template}'.", errors)
			};

			context.Result = new JsonResult(res);
			context.ExceptionHandled = true;
		}
	}
}
