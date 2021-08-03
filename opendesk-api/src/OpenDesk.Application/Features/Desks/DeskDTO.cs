using OpenDesk.Application.ValueObjects;

namespace OpenDesk.Application.Features.Desks
{
	public class DeskDTO
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
	}
}
