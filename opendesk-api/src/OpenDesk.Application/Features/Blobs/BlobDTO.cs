using OpenDesk.Application.Entities;
using System;

namespace OpenDesk.Application.Features.Blobs
{
	public class BlobDTO
	{
		public string Id { get; set; }
		public string Uri { get; set; }
		public DateTimeOffset Expiry { get; set; }

		public BlobDTO()
		{

		}
		public BlobDTO(Blob fromBlob)
		{
			Id = fromBlob.Id;
			Uri = fromBlob.Uri;
			Expiry = fromBlob.Expiry;
		}
	}
}
