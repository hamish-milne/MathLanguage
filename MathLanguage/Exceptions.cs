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

	public class LexerException : RuntimeException
	{
		public LexerException(string message)
			: base(message)
		{
		}
	}

	public class InvalidOperatorException : RuntimeException
	{
		public InvalidOperatorException(Operator op, string a, string b)
			: base("Operator '" + op + "' is invalid for '" + a + "' and '" + b + "'")
		{
		}
	}

	public class MissingMemberException : RuntimeException
	{
		public MissingMemberException(string member, string type)
			: base("Member '" + member + "' does not exist in '" + type + "'")
		{
		}
	}

	public class MemberTypeException : RuntimeException
	{
		public MemberTypeException(string member, string obj, string type, string required)
			: base("Member '" + member + "' in '" + obj + "' cannot be " + type +
			(required == null ? "" : (", it must be " + required)))
		{
		}
	}

	public class IndexException : RuntimeException
	{
		public IndexException(string type, int paramCount)
			: base("A " + type + " value does not have an index with " + paramCount + " parameters")
		{
		}
	}
}
