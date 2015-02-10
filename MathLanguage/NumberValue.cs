using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public abstract class NumberValue : Value
	{
		public abstract Complex AsComplex { get; }

		public abstract double Real { get; }

		public abstract bool IsReal { get; }

		public abstract bool IsDiscrete { get; }

		public abstract long AsDiscrete { get; }

		protected Delta delta = Delta.Exact;

		public virtual Delta Delta
		{
			get { return delta; }
		}
	}
}
