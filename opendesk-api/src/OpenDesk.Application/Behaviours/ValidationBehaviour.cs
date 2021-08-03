using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Behaviours
{
	public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators;
		}

		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			if (_validators.Any())
			{
				List<ValidationFailure> failures = _validators
					.Select(v => v.Validate(request))
					.SelectMany(res => res.Errors)
					.Where(failure => failure != null)
					.ToList();

				if (failures.Any())
				{
					throw new ValidationException("Validation failure.", failures);
				}
			}

			return next();
		}
	}
}