using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class Vector : Value
	{
		NumberValue[] array;

		public override string TypeName
		{
			get { return "vector"; }
		}

		protected Vector(int length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException();
			array = new NumberValue[length];
			for (int i = 0; i < array.Length; i++)
				array[i] = RealValue.Zero;
		}

		public override Value GetIndex(params Value[] indices)
		{
			if (indices.Length != 1)
				base.GetIndex(indices);
			var num = indices[0] as NumberValue;
			long discrete;
			if (num == null || !num.IsDiscrete || (discrete = num.AsDiscrete) < 0)
				throw new IndexTypeException(TypeName);
			if (discrete >= array.Length)
				return Value.None;
			return array[discrete];
		}
	}
}
