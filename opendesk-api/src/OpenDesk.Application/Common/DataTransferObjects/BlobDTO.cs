using OpenDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.DataTransferObjects
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
