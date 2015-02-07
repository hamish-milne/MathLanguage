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
		Negate,
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
		public abstract string TypeName { get; }
	}

}
