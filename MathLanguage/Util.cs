using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MathLanguage
{
	public static class Util
	{
		public delegate bool Compare<T>(T a, T b);

		public static U Max<U>(this IList<U> list, Compare<U> compare)
		{
			U ret = default(U);
			for (int i = 0; i < list.Count; i++)
				if (compare(list[i], ret))
					ret = list[i];
			return ret;
		}
	}
}
