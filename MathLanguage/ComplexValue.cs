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
	}

	public class ComplexValue : Value
	{
		Dictionary<Complex, ComplexValue> cachedValues
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

		public Value Get(Complex value)
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

		delegate Complex Op(Complex a, Complex b);
		static void RegisterOperation(Operator op, Op opr)
		{
			OperatorManager.I.Register<ComplexValue, ComplexValue>(op, (a, b, assign) =>
			{
				if (assign)
				{
					a.Value = opr(a.Value, b.Value);
					return a;
				}
				var newValue = opr(a.Value, b.Value);
				if (newValue == a.Value)
					return a;
				return new ComplexValue(newValue);
			});
		}

		delegate bool Test(Complex a, Complex b);
		static void RegisterComparison(Operator op, Test test)
		{
			OperatorManager.I.Register<ComplexValue, ComplexValue>(op,
				(a, b, assign) => BoolValue.Get(test(a.Value, b.Value)));
		}

		static ComplexValue()
		{
			RegisterOperation(Operator.Add, (a, b) => a + b);
			RegisterOperation(Operator.Subtract, (a, b) => a - b);
			RegisterOperation(Operator.Multiply, (a, b) => a * b);
			RegisterOperation(Operator.Divide, (a, b) => a / b);
			RegisterOperation(Operator.Negate, (a, b) => -a);

		}
	}
}
