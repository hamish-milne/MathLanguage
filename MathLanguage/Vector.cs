using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public abstract class Vector<T> : Value where T : Value
	{
		protected T[] array;
		
		protected delegate Vector<T> Creator(int length);
		protected static Creator creator;

		public static Vector<T> Create(int length)
		{
			if (creator == null)
				throw new InvalidOperationException("No creator set for vector of " + typeof(T));
			return creator(length);
		}

		public int Length
		{
			get { return array.Length; }
		}

		protected Vector(int length)
		{
			array = ArrayPool<T>.Get(length);
		}

		protected abstract Value GetZero();

		protected virtual int GetIndex(string member)
		{
			if (member == null)
				throw new ArgumentNullException();
			switch (member)
			{
				case "x":
					return 0;
				case "y":
					return 1;
				case "z":
					return 2;
				case "w":
					return 3;
				default:
					throw new MissingMemberException(member, TypeName);
			}
		}

		public override Value this[string member]
		{
			get
			{
				int index = GetIndex(member);
				if (index >= Length)
					return GetZero();
				return array[index];
			}
		}

		public override Value SetMember(string member, Value value)
		{
			int index = GetIndex(member);
			if(index >= Length)
			{
				var newVector = Create(index + 1);
				for (int i = 0; i < Length; i++)
					newVector.array[i] = array[i];
				//newVector.array[index] = value;
				return newVector;
			}
			//array[index] = value;
			return null;
		}
	}
}
