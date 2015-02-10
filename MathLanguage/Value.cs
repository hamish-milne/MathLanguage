using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{

	public abstract class Value
	{
		public static readonly Value None = new NoneValue();

		class NoneValue : Value
		{
			public override string TypeName
			{
				get { return "none"; }
			}

			public override Value DoOperation(Operator op, Value right, bool assign)
			{
				if (right is NoneValue)
				switch (op)
				{
					case Operator.Equal:
					case Operator.In:
						return BoolValue.Get(true);
					case Operator.NotEqual:
					case Operator.NotIn:
						return BoolValue.Get(false);
				}
				return null;
			}
		}

		public abstract string TypeName { get; }

		protected IDictionary<string, Value> members;
		public virtual Value this[string member]
		{
			get
			{
				if (member == null)
					throw new ArgumentNullException();
				Value ret = null;
				if (members != null)
					members.TryGetValue(member, out ret);
				if (ret == null)
					throw new MissingMemberException(member, TypeName);
				return ret;
			}
		}

		public virtual Value SetMember(string member, Value value)
		{
			if (member == null)
				throw new ArgumentNullException();
			if (members == null)
				members = new Dictionary<string, Value>();
			members[member] = value;
			return this;
		}

		public virtual Value GetIndex(params Value[] indices)
		{
			throw new IndexException(TypeName, indices.Length);
		}

		public virtual Value SetIndex(Value newValue, params Value[] indices)
		{
			throw new IndexException(TypeName, indices.Length);
		}

		public virtual Value DoOperation(Operator op, Value right, bool assign)
		{
			return null;
		}
	}

}
