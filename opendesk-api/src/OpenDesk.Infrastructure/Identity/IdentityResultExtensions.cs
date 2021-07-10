using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Identity
{
	public static class IdentityResultExtensions
	{
		public static Result ToOpenDeskResult(this IdentityResult result)
		{
			return result.Succeeded
				? Result.Success()
				: Result.Failure(result.Errors.Select(e => e.Description));
		}

		public static Result<T> ToOpenDeskResult<T>(this IdentityResult result, T value)
		{
			return result.Succeeded
				? Result<T>.Success(value)
				: Result<T>.Failure(result.Errors.Select(e => e.Description));
		}
	}
}
