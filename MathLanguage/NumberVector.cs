using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class NumberVector : Vector<NumberValue>
	{
		static NumberVector()
		{
			Create = (length) => new NumberVector(length);
		}

		protected NumberVector(int length) : base(length)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] = RealValue.Zero;
		}

		
	}
}
