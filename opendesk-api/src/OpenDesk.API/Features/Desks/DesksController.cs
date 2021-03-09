using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	[Route("api/office")]
	public class DesksController : ControllerBase
	{
		[HttpGet("{officeId}/desks")]
		public async Task<IActionResult> Get(string officeId)
		{
			return Ok();
		}

		[HttpPost("{officeId}/desks")]
		public async Task<IActionResult> Create(string officeId, [FromBody]CreateDesk.Command command)
		{
			command.OfficeId = officeId;

			return Ok();
		}

		[HttpDelete("{officeId}/desks/{deskId}")]
		public async Task<IActionResult> Delete(string officeId, string deskId)
		{
			return Ok();
		}
	}
}
