using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace MathLanguage
{
	public class Lexer
	{
		protected Dictionary<string, Token> foundTokens
			= new Dictionary<string, Token>()
			{
				{ "let", Token.Let },
				{ "mut", Token.Mut },
				{ "const", Token.Const },

				{ "if", Token.If },
				{ "else", Token.Else },
				{ "for", Token.For },
				{ "while", Token.While },
				{ "switch", Token.Switch },
				{ "break", Token.Break },
				{ "return", Token.Return },

				{ "in", Token.In },
				{ "|", Token.Union },
				{ "&", Token.Intersect },
				{ "!", Token.Not },
				{ "=", Token.Equals },
				{ ">", Token.Greater },
				{ "<", Token.Less },
				{ "+", Token.Plus },
				{ "-", Token.Minus },
				{ "*", Token.Multiply },
				{ "/", Token.Divide },
				{ "%", Token.Modulo },
				{ "^", Token.Power },

				{ "'", Token.Diff },
				{ "(", Token.OpenParen },
				{ ")", Token.CloseParen },
				{ "{", Token.OpenBrace },
				{ "}", Token.CloseBrace },
				{ "[", Token.OpenBracket },
				{ "]", Token.CloseBracket },
				{ ",", Token.Comma },
				{ ".", Token.Dot },
				{ ":", Token.Colon },
				{ ";", Token.Semicolon },
			};

		protected Stream stream;
		protected StreamReader reader;
		protected StringBuilder sb = new StringBuilder();
		protected static char[] buf = new char[1];
		protected int line = 1;
		protected int col = 0;

		public Lexer(Stream stream, Encoding encoding = null)
		{
			if (stream == null)
				throw new ArgumentNullException();
			this.stream = stream;
			reader = encoding == null ?
				new StreamReader(stream, true) :
				new StreamReader(stream, encoding);
		}

		protected Char lastChar = '\0';
		protected virtual Char GetChar()
		{
			if(lastChar != '\0')
			{
				var c = lastChar;
				lastChar = '\0';
				return c;
			}
			// Skip nullbytes in the stream, so we can use them as an EOF marker
			while(reader.Read(buf, 0, 1) == 1 && buf[0] == '\0');
			return buf[0];
		}

		protected void SkipLine()
		{
			while (reader.Read(buf, 0, 1) == 1 && buf[0] != '\n');
			line++;
			col = 0;
		}

		enum Mode
		{
			None,
			Identifier,
			String,
			Escape,
			Number,
		}

		protected virtual bool GetEscapeChar(ref char c)
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

		public virtual TokenData Next()
		{
			sb.Remove(0, sb.Length);
			char c;
			var mode = Mode.None;
			var token = Token.None;
			while((c = GetChar()) != '\0')
			{
				col++;
				bool categorySwitch = false;
				// Some modes don't care what category this char is,
				// so we'll deal with those first
				switch (mode)
				{
					case Mode.String:
						if (c == '\\')
							mode = Mode.Escape;
						else if (c == '"')
							mode = Mode.None;
						else
							sb.Append(c);
						break;
					case Mode.Escape:
						if (!GetEscapeChar(ref c))
							throw new LexerException("Invalid escape character: " + c);
						sb.Append(c);
						mode = Mode.String;
						break;
					default:
						categorySwitch = true;
						break;
				}
				// The category matters for the rest
				if(categorySwitch) switch(Char.GetUnicodeCategory(c))
				{
					case UnicodeCategory.Control:
					case UnicodeCategory.SpaceSeparator:
					case UnicodeCategory.ParagraphSeparator:
					case UnicodeCategory.LineSeparator:
					case UnicodeCategory.Format:
						if(c == '\n')
						{
							line++;
							col = 1;
						}
						if (mode == Mode.None)
							continue;
						break;
					// Identifiers can be any unicode letter or underscores
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.Surrogate:
					case UnicodeCategory.PrivateUse:
					case UnicodeCategory.LetterNumber:
					case UnicodeCategory.NonSpacingMark:
					case UnicodeCategory.SpacingCombiningMark:
						switch (mode)
						{
							case Mode.None:
								mode = Mode.Identifier;
								token = Token.Identifier;
								goto case Mode.Identifier;
							case Mode.Identifier:
								sb.Append(c);
								break;
							default:
								lastChar = c;
								mode = Mode.None;
								break;
						}
						break;
					case UnicodeCategory.DecimalDigitNumber:
						switch(mode)
						{
							case Mode.None:
								mode = Mode.Number;
								token = Token.Number;
								goto case Mode.Number;
							case Mode.Number:
							case Mode.Identifier:
								sb.Append(c);
								break;
							default:
								lastChar = c;
								mode = Mode.None;
								break;
						}
						break;
					case UnicodeCategory.ClosePunctuation:
					case UnicodeCategory.OpenPunctuation:
					case UnicodeCategory.ConnectorPunctuation:
					case UnicodeCategory.DashPunctuation:
					case UnicodeCategory.MathSymbol:
					case UnicodeCategory.OtherPunctuation:
						if(c == '"')
						{
							mode = Mode.String;
							token = Token.String;
							break;
						}
						switch(mode)
						{
							case Mode.None:
								sb.Append(c);
								if(c == '_') // Underscores are identifier chars
								{
									mode = Mode.Identifier;
									token = Token.Identifier;
								} // Everything else is a single char operator
								break;
							case Mode.Number:
								if (c == '.')
									sb.Append(c);
								else goto default;
								break;
							case Mode.Identifier:
								if (c == '_')
									sb.Append(c);
								else goto default;
								break;
							default:
								lastChar = c;
								mode = Mode.None;
								break;
						}
						break;
					default:
						throw new LexerException("Unexpected character: " + c);
				}
				if (mode == Mode.None)
					break;
			}
			TokenData ret;
			var str = sb.ToString();
			// An identifier-type token could be a keyword, so let's check for those
			if(token == Token.Identifier)
			{
				foundTokens.TryGetValue(sb.ToString(), out token);
				if(token == Token.None)
					token = Token.Identifier;
			}
			ret.token = token;
			ret.line = line;
			ret.col = col;
			ret.str = null;
			ret.literal = 0;
			switch(token)
			{
				case Token.Identifier:
				case Token.String:
					ret.str = str;
					break;
				case Token.Number:
					ret.literal = Double.Parse(sb.ToString());
					break;
			}
			return ret;
		}

	}
}
