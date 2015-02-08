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

			static NoneValue()
			{
				Operation<NoneValue, NoneValue> t = (a, b, assign) => BoolValue.Get(true);
				Operation<NoneValue, NoneValue> f = (a, b, assign) => BoolValue.Get(false);
				OperatorManager.I.Register(Operator.Equal, t);
				OperatorManager.I.Register(Operator.NotEqual, f);
				OperatorManager.I.Register(Operator.In, t);
				OperatorManager.I.Register(Operator.NotIn, f);
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
	}

}
