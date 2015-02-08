using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class NumberValue : Value
	{
		static Dictionary<double, NumberValue> cachedValues = new Dictionary<double, NumberValue>()
		{
			{ 0, new NumberValue(0, true) },
			{ Double.Epsilon, new NumberValue(Double.Epsilon, true) },
			{ Double.NaN, new NumberValue(Double.NaN, true) },
			{ Double.PositiveInfinity, new NumberValue(Double.PositiveInfinity, true) },
			{ Double.NegativeInfinity, new NumberValue(Double.NegativeInfinity, true) },
		};

		protected double internalValue;
		protected bool constant;

		public virtual bool Constant
		{
			get { return constant; }
		}

		public virtual double Value
		{
			get { return internalValue; }
		}

		public virtual NumberValue SetValue(double value)
		{
			if (Constant)
				return Get(value);
			internalValue = value;
			return this;
		}

		protected NumberValue(double value, bool constant)
		{
			internalValue = value;
			this.constant = constant;
		}

		public static NumberValue Get(double value)
		{
			NumberValue ret;
			cachedValues.TryGetValue(value, out ret);
			if (ret == null)
				ret = new NumberValue(value, false);
			return ret;
		}

		public override string TypeName
		{
			get { return "real"; }
		}

		public override Value this[string member]
		{
			get
			{
				if (member == null)
					throw new ArgumentNullException();
				switch(member)
				{
					case "x":
					case "Re":
						return this;
					case "y":
					case "z":
					case "w":
					case "Im":
						return Get(0);
				}
				throw new MissingMemberException(member, TypeName);
			}
		}

		public override Value SetMember(string member, Value value)
		{
			if (member == null)
				throw new ArgumentNullException();
			int newComponent;
			switch(member)
			{
				case "x":
				case "Re":
					newComponent = 0;
					break;
				case "y":
					newComponent = 2;
					break;
				case "z":
					newComponent = 3;
					break;
				case "w":
					newComponent = 4;
					break;
				case "Im":
					newComponent = 1;
					break;
				default:
					throw new MissingMemberException(member, TypeName);
			}
			if (value == null)
				value = None;
			if(!(value is NumberValue) && !(value is ComplexValue && newComponent > 1))
				throw new MemberTypeException(member, TypeName, value.TypeName, TypeName);

			var nv = value as NumberValue;
			switch(newComponent)
			{
				case 0:
					return SetValue(nv.Value);
				case 1:
					if (nv.Value == 0.0)
						return this;
					return ComplexValue.Get(new Complex(Value, nv.Value));
				case 2:
				case 3:
				case 4:
					throw new Exception("Vectors not supported yet :(");
			}
			return this;
		}

		delegate double Op(double a, double b);
		static void SelfOp(Operator op, Op opr)
		{
			OperatorManager.I.Register<NumberValue, NumberValue>(op, (a, b, assign) =>
			{
				var newValue = opr(a.Value, b.Value);
				if (newValue == a.Value)
					return a;
				if (assign)
					return a.SetValue(newValue);
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
