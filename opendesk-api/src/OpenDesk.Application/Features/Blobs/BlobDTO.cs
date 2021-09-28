using OpenDesk.Application.Entities;
using System;

namespace OpenDesk.Application.Features.Blobs
{
	public class BlobDto
	{
		public string Id { get; set; }
		public string Uri { get; set; }
		public DateTimeOffset Expiry { get; set; }

		public BlobDto()
		{

		}
		public BlobDto(Blob fromBlob)
		{
			Id = fromBlob.Id;
			Uri = fromBlob.Uri;
			Expiry = fromBlob.Expiry;
		}
	}
}
