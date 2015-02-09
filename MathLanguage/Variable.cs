using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{

	public class Variable
	{
		Value value;
		VariableFlags flags;

		public virtual VariableFlags Flags
		{
			get { return flags; }
			set { flags = value; }
		}

		public virtual Value Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
	}
}
