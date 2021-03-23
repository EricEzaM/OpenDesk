using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Models
{
	public class ApiResponse<T> : ApiResponse
	{
		public T Data { get; set; }

		public ApiResponse() :base() {}

		public ApiResponse(T data) : base()
		{
			Data = data;
		}
	}

	public class ApiResponse
	{
		public ApiResponse()
		{
			Outcome = OperationOutcome.Success;
		}

		public OperationOutcome Outcome { get; set; }
	}

	public class OperationOutcome
	{
		public bool IsError { get; set; }
		public bool IsValidationFailure { get; set; }
		public bool IsSuccess => !IsError && !IsValidationFailure;
		public string Message { get; set; }
		public IEnumerable<string> Errors { get; set; }

		public static OperationOutcome Success => new OperationOutcome
		{
			IsError = false,
			IsValidationFailure = false,
			Errors = Enumerable.Empty<string>(),
			Message = string.Empty,
		};

		public static OperationOutcome Error(IEnumerable<string> errors, string message) => new OperationOutcome
		{
			IsError = true,
			IsValidationFailure = false,
			Errors = errors,
			Message = message,
		};

		public static OperationOutcome ValidationFailure(IEnumerable<string> errors, string message) => new OperationOutcome
		{
			IsError = false,
			IsValidationFailure = true,
			Errors = errors,
			Message = message,
		};
	}
}
