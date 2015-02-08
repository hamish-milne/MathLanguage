using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class ComplexValue : Value
	{
		static readonly Dictionary<Complex, ComplexValue> cachedValues
			= new Dictionary<Complex, ComplexValue>()
		{
			{ Complex.Zero, new ComplexValue(Complex.Zero) },
			{ Complex.I, new ComplexValue(Complex.I) },
		};

		protected Complex internalValue;
		protected bool constant;
		double magnitude = -1;

		public virtual bool Constant
		{
			get { return constant; }
		}

		public virtual double Re
		{
			get { return internalValue.Re; }
		}

		public virtual double Im
		{
			get { return internalValue.Im; }
		}

		public virtual Complex Value
		{
			get { return internalValue; }
		}

		public double Magnitude
		{
			get
			{
				if (magnitude < 0.0)
					magnitude = Value.GetMagnitude();
				return magnitude;
			}
		}

		public virtual Value SetValue(Complex value)
		{
			if (Constant)
				return Get(value);
			internalValue = value;
			magnitude = -1;
			return this;
		}

		public virtual bool Equals(ComplexValue other)
		{
			if (other == null)
				return false;
			return (other.Value == Value);
		}

		protected ComplexValue(Complex value)
		{
			internalValue = value;
		}

		public static Value Get(Complex value)
		{
			if (value.Im == 0)
				return NumberValue.Get(value.Re);
			ComplexValue ret;
			cachedValues.TryGetValue(value, out ret);
			if (ret == null)
				ret = new ComplexValue(value);
			return ret;
		}

		public override string TypeName
		{
			get { return "complex"; }
		}

		static void Register(Operator op, Operation<ComplexValue, ComplexValue> opr)
		{
			OperatorManager.I.Register(op, opr);
		}

		delegate Complex Op(Complex a, Complex b);
		static void SelfOp(Operator op, Op opr)
		{
			Register(op, (a, b, assign) =>
			{
				var newValue = opr(a.Value, b.Value);
				if (newValue == a.Value)
					return a;
				if (assign)
					return a.SetValue(newValue);
				return Get(newValue);
			});
			OperatorManager.I.Register<NumberValue, ComplexValue>(op, (a, b, assign) =>
			{
				var aVal = new Complex(a.Value, 0.0);
				var newValue = opr(aVal, b.Value);
				if (newValue == aVal)
					return a;
				if(assign && newValue.Im == 0.0)
				{
					return a.SetValue(newValue.Re);
				}
				return Get(newValue);
			});
			OperatorManager.I.Register<ComplexValue, NumberValue>(op, (a, b, assign) =>
			{
				var newValue = opr(a.Value, new Complex(b.Value, 0.0));
				if (newValue == a.Value)
					return a;
				if (assign)
					return a.SetValue(newValue);
				return Get(newValue);
			});
		}

		delegate bool Test(Complex a, Complex b);
		static void SelfTest(Operator op, Test test)
		{
			Register(op, (a, b, assign) => BoolValue.Get(test(a.Value, b.Value)));
			OperatorManager.I.Register<NumberValue, ComplexValue>(op,
				(a, b, assign) => BoolValue.Get(test(new Complex(a.Value, 0), b.Value)));
			OperatorManager.I.Register<ComplexValue, NumberValue>(op,
				(a, b, assign) => BoolValue.Get(test(a.Value, new Complex(b.Value, 0))));
		}

		static ComplexValue()
		{
			SelfOp(Operator.Add, (a, b) => a + b);
			SelfOp(Operator.Subtract, (a, b) => a - b);
			SelfOp(Operator.Multiply, (a, b) => a * b);
			SelfOp(Operator.Divide, (a, b) => a / b);
			SelfOp(Operator.Negate, (a, b) => -a);

			Test eq = (a, b) => a == b;
			Test neq = (a, b) => a != b;
			SelfTest(Operator.Equal, eq);
			SelfTest(Operator.In, eq);
			SelfTest(Operator.NotEqual, neq);
			SelfTest(Operator.NotIn, neq);

			Register(Operator.Magnitude,
				(a, b, assign) => NumberValue.Get(a.Magnitude));
			Register(Operator.Substitute, null);
			Register(Operator.Evaluate, null);
		}
	}
}
