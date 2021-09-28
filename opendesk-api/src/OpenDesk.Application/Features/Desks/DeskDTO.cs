using OpenDesk.Application.ValueObjects;

namespace OpenDesk.Application.Features.Desks
{
	public class DeskDto
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
	}
}
