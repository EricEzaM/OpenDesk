using MediatR;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Behaviours
{
	public class StringTrimmingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	{
		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var type = typeof(TRequest);

			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				if (prop.PropertyType == typeof(string) && prop.GetSetMethod() != null && prop.GetValue(request) != null)
				{
					prop.SetValue(request, (prop.GetValue(request) as string).Trim());
				}
			}

			return next();
		}
	}
}
