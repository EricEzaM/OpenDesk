using Microsoft.AspNetCore.Mvc;

namespace OpenDesk.API.Errors
{
	public class EntityNotFoundProblemDetails : ProblemDetails
	{
		public string EntityName { get; set; }
	}
}
