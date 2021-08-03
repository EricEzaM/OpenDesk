using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDesk.Application.Common
{
	public class Result<T>
	{
		internal Result(T value, bool succeeded, IEnumerable<string> errors)
		{
			Value = value;
			Succeeded = succeeded;
			Errors = errors;
		}

		public T Value { get; set; }
		public bool Succeeded { get; set; }
		public IEnumerable<string> Errors { get; }

		public static Result<T> Success(T value)
		{
			return new Result<T>(value, true, Array.Empty<string>());
		}

		public static Result<T> Failure(params string[] errors) => new(default, false, errors);

		public static Result<T> Failure(IEnumerable<string> errors) => new(default, false, errors);
	}

	public class Result
	{
		internal Result(bool succeeded, IEnumerable<string> errors)
		{
			Succeeded = succeeded;
			Errors = errors.ToArray();
		}

		public bool Succeeded { get; set; }
		public string[] Errors { get; set; }

		public static Result Success()
		{
			return new Result(true, Array.Empty<string>());
		}

		public static Result Failure(IEnumerable<string> errors)
		{
			return new Result(false, errors);
		}

		public static Result Failure(params string[] errors)
		{
			return new Result(false, errors);
		}
	}
}
