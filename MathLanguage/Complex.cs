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
			var denom = 1 / ((b.Re * b.Re) + (b.Im * b.Im));
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
}
