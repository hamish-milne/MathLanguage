using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MathLanguage
{
	public class Parser
	{
		public virtual bool Accept(TokenData token)
		{
			return true;
		}
	}
}
