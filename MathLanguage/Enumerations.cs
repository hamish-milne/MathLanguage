using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public enum Delta
	{
		Negative = -1,
		Exact = 0,
		Positive = 1,
	}

	public enum Operator
	{	/*
		// Classes
		Binary = 0x0,
		Test = 0x10,
		Unary = 0x20,
		ClassMask = 0xF0,
		*/
		// Binary
		Add = 0x0,
		Subtract = 0x1,
		Multiply = 0x2,
		Divide = 0x3,
		Power = 0x4,
		Union = 0x5,
		Intersect = 0x6,
		// Comparison
		In = 0x10,
		Equal = 0x11,
		Greater = 0x12,
		Less = 0x13,
		// Combined operators
		NotIn = 0x14,
		NotEqual = 0x15,
		GreaterEqual = 0x16,
		LessEqual = 0x17,
		// Unary
		Not = 0x20,
		Negate = 0x21,
		Magnitude = 0x22,
		Substitute = 0x23,
		Evaluate = 0x24,
	}

	public enum VariableFlags
	{
		None,
		Constant,
		Mutable
	}

}
