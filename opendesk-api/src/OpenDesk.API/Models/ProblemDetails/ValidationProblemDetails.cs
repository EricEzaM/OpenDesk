using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Models.ProblemDetailsExtensions
{
	public class ValidationProblemDetails : ProblemDetails
	{
		public List<ValidationFailure> Errors { get; set; }
	}
}
