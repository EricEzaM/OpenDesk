using FluentValidation;
using MediatR;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Application.Common.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Behaviours
{
	public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IValidatable
	where TResponse : class
	{
		private readonly IValidator<TRequest> _validator;

		public ValidationBehaviour(IValidator<TRequest> validator)
		{
			_validator = validator;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var result = await _validator.ValidateAsync(request);

			if (result.IsValid == false)
			{
				var responseType = typeof(TResponse);

				if (responseType.IsGenericType)
				{
					var resultType = responseType.GetGenericArguments()[0];
					var invalidResponseType = typeof(ValidatableResponse<>).MakeGenericType(resultType);

					var invalidResponse = Activator.CreateInstance(invalidResponseType, null, result.Errors.Select(vf => vf.ErrorMessage).ToList()) as TResponse;

					return invalidResponse;
				}
			}

			var response = await next();

			return response;
		}
	}
}