using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class DiscreteValue : NumberValue
	{
		public static DiscreteValue Zero = new DiscreteValue(0, true);
		public static DiscreteValue One = new DiscreteValue(1, true);

		protected long internalValue;
		bool constant;

		protected DiscreteValue(long value, bool constant)
		{
			internalValue = value;
			this.constant = constant;
		}

		public bool Constant
		{
			get { return constant; }
		}

		public virtual long Value
		{
			get { return internalValue; }
		}

		public DiscreteValue Get(long value)
		{
			if (value == 0)
				return Zero;
			if (value == 1)
				return One;
			return new DiscreteValue(value, false);
		}

		public DiscreteValue SetValue(long newValue)
		{
			if (Value == newValue)
				return this;
			if (Constant)
				return Get(newValue);
			internalValue = newValue;
			return this;
		}

		public override Complex AsComplex
		{
			get { return new Complex((double)Value, 0.0); }
		}

		public override double Real
		{
			get { return (double)Value; }
		}

		public override long AsDiscrete
		{
			get { return Value; }
		}

		public override bool IsReal
		{
			get { return true; }
		}

		public override bool IsDiscrete
		{
			get { return true; }
		}

		public override string TypeName
		{
			get { return "discrete"; }
		}

		public static long Pow(long x, long y)
		{
			long ret = 1;
			while (y-- > 0)
				ret *= x;
			return ret;
		}

		public static long? BinaryOperation(Operator op, long left, long right)
		{
			switch(op)
			{
				case Operator.Add:
					return left + right;
				case Operator.Subtract:
					return left - right;
				case Operator.Multiply:
					return left * right;
				case Operator.Divide:
					return left / right;
				case Operator.Power:
					return Pow(left, right);
				default:
					return null;
			}
		}

		public static bool? Test(Operator op, long left, long right)
		{
			switch(op)
			{
				case Operator.In:
				case Operator.Equal:
					return left == right;
				case Operator.NotIn:
				case Operator.NotEqual:
					return left != right;
				case Operator.Greater:
					return left > right;
				case Operator.Less:
					return left < right;
				case Operator.GreaterEqual:
					return left >= right;
				case Operator.LessEqual:
					return left <= right;
				default:
					return null;
			}
		}

		public static long? Unary(Operator op, long value)
		{
			switch (op)
			{
				case Operator.Evaluate:
				case Operator.Substitute:
					return value;
				case Operator.Magnitude:
					return Math.Abs(value);
				case Operator.Negate:
					return -value;
				default:
					return null;
			}
		}

		public override Value DoOperation(Operator op, Value right, bool assign)
		{
			long? newValue = null;
			bool? boolValue = null;
			if(right == null)
			{
				newValue = Unary(op, Value);
			} else
			{
				var num = right as NumberValue;
				if(num == null)
					return null;
				var other = num.AsDiscrete;
				if(num.IsDiscrete)
				{
					newValue = BinaryOperation(op, Value, other);
					if (newValue == null)
						boolValue = Test(op, Value, other);
				} else if(num.IsReal)
				{
					double? rValue = RealValue.BinaryOperation(op, Real, num.Real);
					if (rValue.HasValue)
						return RealValue.Get(rValue.Value);
					boolValue = RealValue.Test(op, Real, num.Real);
				} else
				{
					var complex = AsComplex;
					Complex? cValue = ComplexValue.BinaryOperation(op, complex, num.AsComplex);
					if (cValue.HasValue)
						return ComplexValue.Get(cValue.Value);
					boolValue = ComplexValue.Test(op, complex, num.AsComplex);
				}
			}
			if (boolValue.HasValue)
				return BoolValue.Get(boolValue.Value);
			if (!newValue.HasValue)
				return null;
			if (assign)
				return SetValue(newValue.Value);
			return Get(newValue.Value);
		}
	}
}
