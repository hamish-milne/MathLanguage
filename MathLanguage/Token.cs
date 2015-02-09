using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public enum Token
	{
		None,
		Identifier,
		Number,
		String,
		EOF,
		// Keywords
		Let,
		Mut,
		Const,
		// Control
		If,
		Else,
		For,
		While,
		Switch,
		Break,
		Return,
		// Binary operators
		In,
		Union,
		Intersect,
		Not,
		Equals,
		Greater,
		Less,
		Plus,
		Minus,
		Multiply,
		Divide,
		Modulo,
		Power,
		// Misc operators
		Diff,
		OpenParen,
		CloseParen,
		OpenBrace,
		CloseBrace,
		OpenBracket,
		CloseBracket,
		Comma,
		Dot,
		Colon,
		Semicolon,
	}

	public struct TokenData
	{
		public Token token;
		public string str;
		public double literal;
		public int line;
		public int col;

		public TokenData(Token token, string str, double literal, int line, int col)
		{
			this.token = token;
			this.str = str;
			this.literal = literal;
			this.line = line;
			this.col = col;
		}
	}
}
