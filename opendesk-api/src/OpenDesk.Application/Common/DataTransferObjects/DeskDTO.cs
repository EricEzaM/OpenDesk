using OpenDesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.DataTransferObjects
{
	public class DeskDTO
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
	}
}
