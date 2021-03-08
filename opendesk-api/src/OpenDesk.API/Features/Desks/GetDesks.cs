using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	public class GetDesks
	{
		[Route("api/desks")]
		public class GetDesksController : ControllerBase
		{
			private readonly IMediator _mediator;

			public GetDesksController(IMediator mediator)
			{
				_mediator = mediator;
			}

			[HttpGet("desks")]
			public async Task<IActionResult> GetDesks()
			{

			}
		}

		public class GetDesks : IRequest
		{

		}

		public class GetDesksDTO
		{

		}
	}
}
