using OpenDesk.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.ValueObjects
{
	public class DiagramPosition : ValueObject
	{
		public DiagramPosition()
		{

		}

		public DiagramPosition(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return X;
			yield return Y;
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}
