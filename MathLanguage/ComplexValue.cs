using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public struct Complex
	{
		public static readonly Complex Zero = new Complex(0, 0);
		public static readonly Complex I = new Complex(0, 1);

		public double Re;
		public double Im;

		public double GetMagnitude()
		{
			return Math.Sqrt((Re * Re) + (Im * Im));
		}

		public Complex(double re, double im)
		{
			Re = re;
			Im = im;
		}

		public static bool operator ==(Complex a, Complex b)
		{
			return (a.Re == b.Re) && (a.Im == b.Im);
		}

		public static bool operator !=(Complex a, Complex b)
		{
			return (a.Re != b.Re) || (a.Im != b.Im);
		}

		public static Complex operator +(Complex a, Complex b)
		{
			return new Complex(a.Re + b.Re, a.Im + b.Im);
		}

		public static Complex operator -(Complex a, Complex b)
		{
			return new Complex(a.Re - b.Re, a.Im - b.Im);
		}

		public static Complex operator *(Complex a, Complex b)
		{
			return new Complex((a.Re * b.Re) - (a.Im * b.Im),
				(a.Re * b.Im) + (a.Im * b.Re));
		}

		public static Complex operator -(Complex a)
		{
			return new Complex(-a.Re, -a.Im);
		}

		public static Complex operator /(Complex a, Complex b)
		{
			var denom = 1/((b.Re * b.Re) + (b.Im * b.Im));
			return new Complex(((a.Re * b.Re) + (a.Im * b.Im)) * denom,
				((a.Im * b.Re) - (a.Re * b.Im)) * denom);
		}

		public override int GetHashCode()
		{
 			return Util.Pair(Re.GetHashCode(), Im.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Complex))
				return false;
			return this == (Complex)obj;
		}
	}

	public class ComplexValue : Value
	{
		static readonly Dictionary<Complex, ComplexValue> cachedValues
			= new Dictionary<Complex, ComplexValue>()
		{
			{ Complex.Zero, new ComplexValue(Complex.Zero) },
			{ Complex.I, new ComplexValue(Complex.I) },
		};

		Complex val;
		double magnitude = -1;

		public double Re
		{
			get { return val.Re; }
			set
			{
				val.Re = value;
				magnitude = -1;
			}
		}

		public double Im
		{
			get { return val.Im; }
			set
			{
				val.Im = value;
				magnitude = -1;
			}
		}

		public Complex Value
		{
			get { return val; }
			set
			{
				val = value;
				magnitude = -1;
			}
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

		public bool Equals(ComplexValue other)
		{
			if (other == null)
				return false;
			return (other.Re == Re) && (other.Im == Im);
		}

		protected ComplexValue(Complex value)
		{
			val = value;
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
				if(assign)
					a.Value = newValue;
				if (assign || newValue == a.Value)
					return a;
				return Get(newValue);
			});
			OperatorManager.I.Register<NumberValue, ComplexValue>(op, (a, b, assign) =>
			{
				var aVal = new Complex(a.Value, 0);
				var newValue = opr(aVal, b.Value);
				if (newValue == aVal)
					return a;
				if(assign && newValue.Im == 0)
				{
					a.Value = newValue.Re;
					return a;
				}
				return Get(newValue);
			});
			OperatorManager.I.Register<ComplexValue, NumberValue>(op, (a, b, assign) =>
			{
				var newValue = opr(a.Value, new Complex(b.Value, 0));
				if (assign)
					a.Value = newValue;
				if (assign || newValue == a.Value)
					return a;
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
