using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.Models
{
	public class ValidatableResponse
	{
		private readonly IList<string> _errors;

		public ValidatableResponse(IList<string> errors = null)
		{
			_errors = errors ?? new List<string>();
		}

		public bool IsValidResponse => !_errors.Any();

		public IReadOnlyCollection<string> Errors => new ReadOnlyCollection<string>(_errors);
	}

	public class ValidatableResponse<T> : ValidatableResponse where T : class
	{
		public ValidatableResponse(T result, IList<string> validationErrors = null) : base(validationErrors)
		{
			Result = result;
		}

		public T Result { get; set; }
	}
}
