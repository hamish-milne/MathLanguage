using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class RuntimeException : Exception
	{
		public RuntimeException(string message) : base(message)
		{
		}
	}

	public class InvalidOperatorException : RuntimeException
	{
		public InvalidOperatorException(Operator op, string a, string b)
			: base("Operator " + op + " is invalid for " + a + " and " + b)
		{
		}
	}
}
