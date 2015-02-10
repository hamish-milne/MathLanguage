using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class ComplexValue : NumberValue
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

		public bool Constant
		{
			get { return constant; }
		}

		public override double Real
		{
			get { return internalValue.Re; }
		}

		public virtual double Imaginary
		{
			get { return internalValue.Im; }
		}

		public virtual Complex Value
		{
			get { return internalValue; }
		}

		public override Complex AsComplex
		{
			get { return Value; }
		}

		public override bool IsReal
		{
			get { return Value.Im == 0.0; }
		}

		public override bool IsDiscrete
		{
			get { return (Imaginary == 0.0) && ((double)(long)Real == Real); }
		}

		public override long AsDiscrete
		{
			get { return (long)Real; }
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
			if (Value == value)
				return this;
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
			if (value.Im == 0.0)
				return RealValue.Get(value.Re);
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

		public static Complex? BinaryOperation(Operator op, Complex left, Complex right)
		{
			switch (op)
			{
				case Operator.Add:
					return left + right;
				case Operator.Subtract:
					return left - right;
				case Operator.Multiply:
					return left * right;
				case Operator.Divide:
					return left / right;
				default:
					return null;
			}
		}

		public static bool? Test(Operator op, Complex left, Complex right)
		{
			switch (op)
			{
				case Operator.In:
				case Operator.Equal:
					return left == right;
				case Operator.NotIn:
				case Operator.NotEqual:
					return left != right;
				default:
					return null;
			}
		}

		public override Value DoOperation(Operator op, Value right, bool assign)
		{
			var number = right as NumberValue;
			Complex? newValue = null;
			bool? boolValue = null;
			if (number == null)
			{
				switch(op)
				{
					case Operator.Negate:
						newValue = -Value;
						break;
					case Operator.Substitute:
					case Operator.Evaluate:
						return this;
					case Operator.Magnitude:
						return RealValue.Get(Magnitude);
				}
			} else
			{
				newValue = BinaryOperation(op, Value, number.AsComplex);
				if (newValue == null)
					boolValue = Test(op, Value, number.AsComplex);
			}
			if (boolValue.HasValue)
				return BoolValue.Get(boolValue.Value);
			else if (newValue.HasValue)
				return assign ? SetValue(newValue.Value) : Get(newValue.Value);
			return null;
		}
	}
}
