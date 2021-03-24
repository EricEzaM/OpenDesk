using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Models
{
	public class ApiResponse<T> : ApiResponse where T : class
	{
		public T Data { get; set; }

		public ApiResponse() :base() {}

		public ApiResponse(T data) : base()
		{
			Data = data;
		}

		public ApiResponse(T data, OperationOutcome outcome) : base(outcome)
		{
			Data = data;
		}

		public ApiResponse(OperationOutcome outcome) : this(null, outcome) { }
	}

	public class ApiResponse
	{
		public ApiResponse()
		{
			Outcome = OperationOutcome.Success;
		}

		public ApiResponse(OperationOutcome outcome)
		{
			Outcome = outcome;
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

		public static OperationOutcome Error(string message) => Error(message, Enumerable.Empty<string>());

		public static OperationOutcome Error(string message, IEnumerable<string> errors) => new OperationOutcome
		{
			IsError = true,
			IsValidationFailure = false,
			Errors = errors,
			Message = message,
		};

		public static OperationOutcome ValidationFailure(string message) => ValidationFailure(message, Enumerable.Empty<string>());

		public static OperationOutcome ValidationFailure(string message, IEnumerable<string> errors) => new OperationOutcome
		{
			IsError = false,
			IsValidationFailure = true,
			Errors = errors,
			Message = message,
		};
	}
}
