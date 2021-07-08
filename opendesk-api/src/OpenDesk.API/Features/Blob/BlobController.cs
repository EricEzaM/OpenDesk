using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Blob
{
	[Authorize]
	[Route("api/blobs")]
	[ApiController]
	public class BlobController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BlobController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{blobId}")]
		public async Task<IActionResult> Get(string blobId)
		{
			var result = await _mediator.Send(new GetBlobCommand(blobId));
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromForm] CreateBlobCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
}
