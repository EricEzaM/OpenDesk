using MediatR;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	public class CreateDesk
	{
		public class Command : IRequest<DeskDTO>
		{
			public string Name { get; set; }
			public DiagramPosition DiagramPosition { get; set; }
			public string OfficeId { get; set; }
		}
	}
}
