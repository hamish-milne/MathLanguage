using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class RealValue : NumberValue
	{
		public static readonly RealValue Zero = new RealValue(0.0, true);

		static Dictionary<double, RealValue> cachedValues = new Dictionary<double, RealValue>()
		{
			{ 0.0, Zero },
			{ Double.Epsilon, new RealValue(Double.Epsilon, true) },
			{ Double.NaN, new RealValue(Double.NaN, true) },
			{ Double.PositiveInfinity, new RealValue(Double.PositiveInfinity, true) },
			{ Double.NegativeInfinity, new RealValue(Double.NegativeInfinity, true) },
			{ 1, new RealValue(1, true) },
			{ -1, new RealValue(-1, true) },
		};

		protected double internalValue;
		protected bool constant;

		public bool Constant
		{
			get { return constant; }
		}

		public virtual double Value
		{
			get { return internalValue; }
		}

		public override double Real
		{
			get { return internalValue; }
		}

		public override Complex AsComplex
		{
			get { return new Complex(Value, 0.0); }
		}

		public override bool IsReal
		{
			get { return false; }
		}

		public virtual RealValue SetValue(double value)
		{
			if (Value == value)
				return this;
			if (Constant)
				return Get(value);
			internalValue = value;
			return this;
		}

		public override bool IsDiscrete
		{
			get { return (Value - (double)(long)Value) == 0.0; }
		}

		public override long AsDiscrete
		{
			get { return (long)Value; }
		}

		protected RealValue(double value, bool constant)
		{
			internalValue = value;
			this.constant = constant;
		}

		public static RealValue Get(double value)
		{
			RealValue ret;
			cachedValues.TryGetValue(value, out ret);
			if (ret == null)
				ret = new RealValue(value, false);
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
				switch (member)
				{
					case "x":
					case "Re":
						return this;
					case "y":
					case "z":
					case "w":
					case "Im":
						return Zero;
				}
				throw new MissingMemberException(member, TypeName);
			}
		}

		public override Value SetMember(string member, Value value)
		{
			if (member == null)
				throw new ArgumentNullException();
			int newComponent;
			switch (member)
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
			var num = value as NumberValue;
			if (num == null || (!num.IsReal && newComponent <= 1))
				throw new MemberTypeException(member, TypeName, value.TypeName, "number");

			var nv = value as RealValue;
			switch (newComponent)
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
					var vector = Vector<NumberValue>.Create(newComponent);
					vector[0] = this;
					vector[newComponent] = num;
					return vector;
			}
			return this;
		}

		public static double? BinaryOperation(Operator op, double left, double right)
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
				case Operator.Power:
					return Math.Pow(left, right);
				default:
					return null;
			}
		}

		public static bool? Test(Operator op, double left, double right)
		{
			switch (op)
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

		public static double? Unary(Operator op, double value)
		{
			switch(op)
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
			double? newValue = null;
			bool? boolResult = null;
			if(right == null)
			{
				newValue = Unary(op, Value);
			} else
			{
				var num = right as NumberValue;
				if (num == null)
					return null;
				if(!num.IsReal)
				{
					Complex? cValue = ComplexValue.BinaryOperation(op, AsComplex, num.AsComplex);
					if (cValue.HasValue)
						return ComplexValue.Get(cValue.Value);
					boolResult = ComplexValue.Test(op, AsComplex, num.AsComplex);
				} else
				{
					newValue = BinaryOperation(op, Value, num.Real);
					if (newValue == null)
						boolResult = Test(op, Value, num.Real);
				}
			}
			if (boolResult.HasValue)
				return BoolValue.Get(boolResult.Value);
			if (!newValue.HasValue)
				return null;
			if (assign)
				return SetValue(newValue.Value);
			return Get(newValue.Value);
		}
	}
}
