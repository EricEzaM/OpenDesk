using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Errors
{
	public class FluentValidationProblemDetails : ProblemDetails
	{
		public FluentValidationProblemDetails(IEnumerable<FluentValidationProblemDetailsError> errors)
		{
			Errors = errors;
		}

		public IEnumerable<FluentValidationProblemDetailsError> Errors { get; set; }
	}

	public class FluentValidationProblemDetailsError
	{
		public FluentValidationProblemDetailsError(ValidationFailure fromFailure)
		{
			Message = fromFailure.ErrorMessage;
			ErrorCode = fromFailure.ErrorCode;
			AttemptedValue = fromFailure.AttemptedValue;
			State = fromFailure.CustomState;
			Severity = fromFailure.Severity;
		}

		public string Message { get; set; }
		public string ErrorCode{ get; set; }
		public object AttemptedValue { get; set; }
		public object State { get; set; }
		public Severity Severity { get; set; }
	}
}
