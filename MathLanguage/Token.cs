using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	enum TokenType
	{
		None,
		Identifier,
		Operator,
		Number,
		String,
		Unknown
	}

	class Token
	{
		string text;
		TokenType type;
		
		public string Text { get { return text; } }

		public TokenType Type { get { return type; } }

		public Token(string text, TokenType type)
		{
			this.text = text;
			this.type = type;
		}
	}
}
