using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public static class ArrayPool<T>
	{
		static int poolSize = 128;
		static int maxItemSize = 64;
		static List<T[]> pool = new List<T[]>();
		static T[] empty = new T[0];

		public static int PoolSize
		{
			get { return poolSize; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				poolSize = value;
			}
		}

		public static int MaxItemSize
		{
			get { return maxItemSize; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				maxItemSize = value;
			}
		}

		public static T[] Get(int length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException();
			if (length == 0)
				return empty;
			T[] ret;
			for(int i = pool.Count - 1; i >= 0; i--)
			{
				ret = pool[i];
				if(ret.Length == length)
				{
					pool.RemoveAt(i);
					return ret;
				}
			}
			ret = new T[length];
			return ret;
		}

		public static void Release(T[] array)
		{
			if (array == null || array.Length == 0)
				return;
			if (pool.Count >= poolSize || array.Length > maxItemSize)
				return;
			pool.Add(array);
		}
	}
}
