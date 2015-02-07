using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public enum Operator
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Power,
		Union,
		Intersect,
		Not,
		Negate,
		// Comparison
		In,
		Equal,
		Greater,
		Less,
		// Combined operators
		NotIn,
		NotEqual,
		GreaterEqual,
		LessEqual,
		// Surrounds
		Magnitude,
		Substitute,
		Evaluate
	}

	public abstract class Value
	{
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

		public static readonly Value None = new NoneValue();
	}

}
