using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace MathLanguage
{
	class TokenException : Exception
	{
		public TokenException(string message)
			: base(message)
		{
		}
	}

	class Tokenizer
	{
		Dictionary<string, Token> foundTokens
			= new Dictionary<string, Token>();

		Stream stream;
		StreamReader reader;
		StringBuilder sb = new StringBuilder();
		static char[] buf = new char[1];

		public Tokenizer(Stream stream, Encoding encoding = null)
		{
			if (stream == null)
				throw new ArgumentNullException();
			this.stream = stream;
			reader = encoding == null ?
				new StreamReader(stream, true) :
				new StreamReader(stream, encoding);
		}

		public void Add(Token t)
		{
			if (t == null)
				throw new ArgumentNullException();
			if (t.Text != null)
				foundTokens[t.Text] = t;
		}

		Char GetChar()
		{
			// Skip nullbytes in the stream, so we can use them as an EOF marker
			while(reader.Read(buf, 0, 1) == 1 && buf[0] == '\0');
			return buf[0];
		}

		void SkipLine()
		{
			while (reader.Read(buf, 0, 1) == 1 && buf[0] != '\n');
		}

		enum Mode
		{
			None,
			Identifier,
			Operator,
			String,
			Escape,
		}

		public bool GetEscapeChar(ref char c)
		{
			switch(c)
			{
				case 'n': c = '\n'; break;
				case 'r': c = '\r'; break;
				case '0': c = '\0'; break;
				case 'f': c = '\f'; break;
				case 't': c = '\t'; break;
				case '\\': c = '\\'; break;
				default: return false;
			}
			return true;
		}

		public Token Next()
		{
			sb.Remove(0, sb.Length);
			Char c;
			var mode = Mode.None;
			var tokenType = TokenType.None;
			while((c = GetChar()) != '\0')
			{
				Char.GetUnicodeCategory(c)
				if (Char.IsWhiteSpace(c))
				{
					if (sb.Length > 0)
						break;
					continue;
				}
				else if (Char.IsDigit(c))
				{

				}
				else if (Char.IsLetter(c))
				{
					switch (mode)
					{
						case Mode.None:
							mode = Mode.Identifier;
							tokenType = TokenType.Identifier;
							goto case Mode.Identifier;
						case Mode.Identifier:
							sb.Append(c);
							break;
						case Mode.Operator:
							mode = Mode.None;
							break;
						case Mode.Escape:
							if (!GetEscapeChar(ref c))
								throw new TokenException("Invalid escape character: " + c);
							sb.Append(c);
							mode = Mode.String;
							break;
						case Mode.String:
							if (c == '\\')
								mode = Mode.Escape;
							else if (c == '"')
								mode = Mode.None;
							else
								sb.Append(c);
							break;
					}
					if (mode == Mode.None)
						break;
				}
			}
			if (tokenType == TokenType.None)
				return null;
			Token ret = null;
			var str = sb.ToString();
			if(tokenType != TokenType.String)
				foundTokens.TryGetValue(str, out ret);
			if (ret == null)
				ret = new Token(str, tokenType);
			return ret;
		}

	}
}
