using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class BoolValue : Value
	{
		readonly bool internalValue;
		static BoolValue True = new BoolValue(true);
		static BoolValue False = new BoolValue(false);

		public bool Value
		{
			get { return internalValue; }
		}

		public override string TypeName
		{
			get { return "bool"; }
		}

		public override T ConvertTo<T>()
		{
			throw new InvalidCastException();
		}

		protected BoolValue(bool value)
		{
			internalValue = value;
		}

		public static BoolValue Get(bool value)
		{
			return value ? True : False;
		}

		delegate bool Op(bool a, bool b);
		static void SelfOp(Operator op, Op opr)
		{
			OperatorManager.I.Register<BoolValue, BoolValue>
				(Operator.Add, (a, b, assign) => Get(opr(a.Value, b.Value)));
		}

		static BoolValue()
		{
			Op or = (a, b) => (a || b);
			Op and = (a, b) => (a && b);
			Op eq = (a, b) => (a == b);
			Op neq = (a, b) => (a != b);
			SelfOp(Operator.Add, or);
			SelfOp(Operator.Union, or);
			SelfOp(Operator.Multiply, and);
			SelfOp(Operator.Intersect, and);
			SelfOp(Operator.Equal, eq);
			SelfOp(Operator.NotEqual, neq);
			SelfOp(Operator.In, eq);
			SelfOp(Operator.NotIn, neq);
			SelfOp(Operator.Power, (a, b) => (a ^ b));
			SelfOp(Operator.Not, (a, b) => (!a));
			SelfOp(Operator.Greater, (a, b) => (a && !b));
			SelfOp(Operator.GreaterEqual, (a, b) => (a || !b));
			SelfOp(Operator.Less, (a, b) => (!a && b));
			SelfOp(Operator.LessEqual, (a, b) => (!a || b));
			OperatorManager.I.Register<BoolValue, BoolValue>(Operator.Magnitude, null);
			OperatorManager.I.Register<BoolValue, BoolValue>(Operator.Substitute, null);
			OperatorManager.I.Register<BoolValue, BoolValue>(Operator.Evaluate, null);
		}
	}
}
