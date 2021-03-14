using OpenDesk.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.ValueObjects
{
	public class OfficeImage : ValueObject
	{
		public OfficeImage()
		{

		}

		public string Url { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Url;
			yield return Width;
			yield return Height;
		}
	}
}
