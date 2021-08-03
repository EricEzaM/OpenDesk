using OpenDesk.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Common.ValueObjects
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

		public DiagramPosition(DiagramPosition other)
		{
			X = other.X;
			Y = other.Y;
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
