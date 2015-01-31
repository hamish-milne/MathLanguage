using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MathLanguage
{
	class Parser
	{
		Stream stream;

		public Parser(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
		}
	}
}
