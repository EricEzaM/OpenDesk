﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Office
{
	public class GetOffices
	{
		public class Command : IRequest<IEnumerable<OfficeDTO>> { }

		public class Handler : IRequestHandler<Command, IEnumerable<OfficeDTO>>
		{
			private readonly OpenDeskDbContext _db;

			public Handler(OpenDeskDbContext db)
			{
				_db = db;
			}

			public async Task<IEnumerable<OfficeDTO>> Handle(Command request, CancellationToken cancellationToken)
			{
				return await _db.Offices
					.Select(ol => new OfficeDTO
					{
						Id = ol.Id,
						Name = ol.Name
					})
					.ToListAsync();
			}
		}
	}

}
