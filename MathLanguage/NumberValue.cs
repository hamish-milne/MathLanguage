using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class NumberValue : Value
	{
		static Dictionary<double, NumberValue> cachedValues = new Dictionary<double, NumberValue>()
		{
			{ 0, new NumberValue(0) },
			{ Double.Epsilon, new NumberValue(Double.Epsilon) },
			{ Double.NaN, new NumberValue(Double.NaN) },
			{ Double.PositiveInfinity, new NumberValue(Double.PositiveInfinity) },
			{ Double.NegativeInfinity, new NumberValue(Double.NegativeInfinity) },
		};

		double internalValue;

		public double Value
		{
			get { return internalValue; }
			set { internalValue = value; }
		}

		protected NumberValue(double value)
		{
			internalValue = value;
		}

		public static NumberValue Get(double value)
		{
			NumberValue ret;
			cachedValues.TryGetValue(value, out ret);
			if (ret == null)
				ret = new NumberValue(value);
			return ret;
		}

		public override string TypeName
		{
			get { return "real"; }
		}

		delegate double Op(double a, double b);
		static void SelfOp(Operator op, Op opr)
		{
			OperatorManager.I.Register<NumberValue, NumberValue>(op, (a, b, assign) =>
			{
				var newValue = opr(a.Value, b.Value);
				if (assign)
					a.Value = newValue;
				if (assign || newValue == a.Value)
					return a;
				return Get(newValue);
			});
		}

		delegate bool Test(double a, double b);
		static void SelfTest(Operator op, Test test)
		{
			OperatorManager.I.Register<NumberValue, NumberValue>(op,
				(a, b, assign) => BoolValue.Get(test(a.Value, b.Value)));
		}

		static NumberValue()
		{
			SelfOp(Operator.Add, (a, b) => a + b);
			SelfOp(Operator.Subtract, (a, b) => a - b);
			SelfOp(Operator.Multiply, (a, b) => a * b);
			SelfOp(Operator.Divide, (a, b) => a / b);
			SelfOp(Operator.Negate, (a, b) => -a);
			SelfOp(Operator.Power, (a, b) => Math.Pow(a, b));

			Test eq = (a, b) => a == b;
			Test neq = (a, b) => a != b;
			SelfTest(Operator.Equal, eq);
			SelfTest(Operator.NotEqual, neq);
			SelfTest(Operator.In, eq);
			SelfTest(Operator.NotIn, neq);
			SelfTest(Operator.Greater, (a, b) => a > b);
			SelfTest(Operator.Less, (a, b) => a < b);
			SelfTest(Operator.GreaterEqual, (a, b) => a >= b);
			SelfTest(Operator.LessEqual, (a, b) => a <= b);
			OperatorManager.I.Register<NumberValue, NumberValue>(Operator.Magnitude, null);
			OperatorManager.I.Register<NumberValue, NumberValue>(Operator.Substitute, null);
			OperatorManager.I.Register<NumberValue, NumberValue>(Operator.Evaluate, null);
		}
	}
}
