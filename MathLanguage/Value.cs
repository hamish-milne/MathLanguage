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
		public abstract T ConvertTo<T>();

		public abstract string TypeName { get; }
	}

	public class BoolValue : Value
	{
		bool internalValue;
		static BoolValue True = new BoolValue(true);
		static BoolValue False = new BoolValue(false);

		public bool Value
		{
			get { return internalValue; }
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
		static void RegisterOperation(Operator op, Op opr)
		{
			OperatorManager.I.Register<BoolValue, BoolValue>
				(Operator.Add, (a, b, assign) => Get(opr(a.Value, b.Value)));
		}

		static BoolValue()
		{
			Op nop = (a, b) => a;
			Op or = (a, b) => (a || b);
			Op and = (a, b) => (a && b);
			Op eq = (a, b) => (a == b);
			Op neq = (a, b) => (a != b);
			RegisterOperation(Operator.Add, or);
			RegisterOperation(Operator.Union, or);
			RegisterOperation(Operator.Multiply, and);
			RegisterOperation(Operator.Union,  and);
			RegisterOperation(Operator.Equal, eq);
			RegisterOperation(Operator.NotEqual, neq);
			RegisterOperation(Operator.In, eq);
			RegisterOperation(Operator.NotIn, neq);
			RegisterOperation(Operator.Power, (a, b) => (a ^ b));
			RegisterOperation(Operator.Not, (a, b) => (!a));
			RegisterOperation(Operator.Greater, (a, b) => (a && !b));
			RegisterOperation(Operator.GreaterEqual, (a, b) => (a || !b));
			RegisterOperation(Operator.Less, (a, b) => (!a && b));
			RegisterOperation(Operator.LessEqual, (a, b) => (!a || b));
			RegisterOperation(Operator.Magnitude, nop);
			RegisterOperation(Operator.Substitute, nop);
			RegisterOperation(Operator.Evaluate, nop);
		}
	}

	public class NumberValue : Value
	{
		double internalValue;
		object boxedValue;

		public double Value
		{
			get { return internalValue; }
			set
			{
				internalValue = value;
				boxedValue = null;
			}
		}

		public override T ConvertTo<T>()
		{
			if(typeof(T).IsPrimitive)
			{
				if (boxedValue == null)
					boxedValue = internalValue;
				return (T)boxedValue;
			}
			throw new InvalidCastException();
		}

		public NumberValue(double value)
		{
			internalValue = value;
		}

		delegate double Op(double a, double b);
		static void RegisterOperation(Operator op, Op opr)
		{
			OperatorManager.I.Register<NumberValue, NumberValue>(op, (a, b, assign) =>
			{
				if (assign)
				{
					a.Value = opr(a.Value, b.Value);
					return a;
				}
				return new NumberValue(opr(a.Value, b.Value));
			});
		}

		delegate bool Test(double a, double b);
		static void RegisterComparison(Operator op, Test test)
		{
			OperatorManager.I.Register<NumberValue, NumberValue>(op,
				(a, b, assign) => BoolValue.Get(test(a.Value, b.Value)));
		}

		static NumberValue()
		{
			RegisterOperation(Operator.Add, (a, b) => a + b);
			RegisterOperation(Operator.Subtract, (a, b) => a - b);
			RegisterOperation(Operator.Multiply, (a, b) => a * b);
			RegisterOperation(Operator.Divide, (a, b) => a / b);
			RegisterOperation(Operator.Power, (a, b) => Math.Pow(a, b));

			Test eq = (a, b) => a == b;
			Test neq = (a, b) => a != b;
			RegisterComparison(Operator.Equal, eq);
			RegisterComparison(Operator.NotEqual, neq);
			RegisterComparison(Operator.In, eq);
			RegisterComparison(Operator.NotIn, neq);
			RegisterComparison(Operator.Greater, (a, b) => a > b);
			RegisterComparison(Operator.Less, (a, b) => a < b);
			RegisterComparison(Operator.GreaterEqual, (a, b) => a >= b);
			RegisterComparison(Operator.LessEqual, (a, b) => a <= b);
		}
	}
}
