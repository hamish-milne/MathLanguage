using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	[InitializeOnLoad]
	public abstract class Vector<T> : Value where T : Value
	{
		public delegate Vector<T> Creator(int length);

		static Creator create;
		public static Creator Create
		{
			get
			{
				if (create == null)
					throw new NullReferenceException("Vector of " + typeof(T) + " has no Creator");
				return create;
			}
			protected set { create = value; }
		}
		
		protected readonly T[] array;

		public virtual int Length
		{
			get { return array.Length; }
		}

		protected Vector(int length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException();
			array = ArrayPool<T>.Get(length);
		}

		~Vector()
		{
			ArrayPool<T>.Release(array);
		}

		public override string TypeName
		{
			get { return "vector"; }
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

		public virtual T this[int index]
		{
			get { return array[index]; }
			set { array[index] = value; }
		}
	}
}
