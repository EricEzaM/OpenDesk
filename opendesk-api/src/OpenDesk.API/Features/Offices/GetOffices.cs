﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	public class GetOfficesCommand : IRequest<ApiResponse<IEnumerable<OfficeDTO>>> { }

	public class GetOfficesHandler : IRequestHandler<GetOfficesCommand, ApiResponse<IEnumerable<OfficeDTO>>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficesHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<ApiResponse<IEnumerable<OfficeDTO>>> Handle(GetOfficesCommand request, CancellationToken cancellationToken)
		{
			return new ApiResponse<IEnumerable<OfficeDTO>>(
				await _db.Offices
				.Select(o => new OfficeDTO
				{
					Id = o.Id,
					Name = o.Name,
					Image = o.Image
				})
				.AsNoTracking()
				.ToListAsync());
		}
	}
}
