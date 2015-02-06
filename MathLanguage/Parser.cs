using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MathLanguage
{
	class Parser
	{
		Stream stream;
		Lexer lexer;

		static string[] keywords = new string[]
		{
			"let",      // Declares a variable
			"mut",      // Allows a variable to be modified in child scopes
			"const",    // Prevents a variable from being modified
			"discrete", // The set of integers
			"none",     // Represents no value
			"i", "j",   // The imaginary constant
			"inf",      // Infinity
			"vector",   // The vector constructor
			"V",        // Shorthand for the above
			"rand",     // Outputs a random number
			"dot",      // Vector dot product
			"cross",    // Vector cross product
			"scale",    // Vector linear scale
			"root",     // nth root
			"x",        // Vector first element
			"y",        // Vector second element
			"z",        // Vector third element
			"w",        // Vector fourth element
		};

		static string[] binaryOperators = new string[]
		{
			"+",	// Sum
			"-",	// Difference
			"*",	// Product
			"/",	// Quotient
			"%",	// Remainder
			"^",	// Power / XOR
			"&",	// Intersection / AND
			"|",	// Union / OR
		};

		static string[] comparisonOperators = new string[]
		{
			"==",	// Equality
			"!=",	// Inequality
			">",	// Greater than
			"<",	// Less than
			">=",	// Not less then
			"<=",	// Not greater than
		};

		static string[] brackets = new string[]
		{
			"(", ")",	// Parens - function calls
			"{", "}",	// Braces - scopes, functions
			"[", "]",	// Brackets - evaluation, vectors
		};

		static string[] unaryOperators = new string[]
		{
			"!",	// Set inverse / NOT
			"'",	// Differential
		};

		static string[] miscOperators = new string[]
		{
			",",	// Separates arguments
			";",	// Ends a statement
			"\"",	// String quotes
			"in",	// Set contains
			":",	// Range
		};

		public Parser(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			lexer = new Lexer(stream);
			
		}
	}
}
