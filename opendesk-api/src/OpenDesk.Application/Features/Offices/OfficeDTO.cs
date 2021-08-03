using OpenDesk.Application.Features.Blobs;

namespace OpenDesk.Application.Features.Offices
{
	public class OfficeDTO
	{
		public string Id { get; set; }
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
		public BlobDTO Image { get; set; }
	}
}
