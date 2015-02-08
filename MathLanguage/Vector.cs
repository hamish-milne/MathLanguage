using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class Vector<T> where T : Value
	{
		T[] array;
		
		public int Length
		{
			get { return array == null ? 0 : array.Length; }
		}

		
	}
}
