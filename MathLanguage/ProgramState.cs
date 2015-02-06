using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public class ProgramState
	{
		Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		public IDictionary<string, Variable> Variables
		{
			get { return variables; }
		}
	}
}
