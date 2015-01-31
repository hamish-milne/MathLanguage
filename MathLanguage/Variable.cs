using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	enum VariableType
	{
		None,
		Scalar,
		Vector,
		Function,
		Set,
	}

	class Variable
	{
		VariableType type;
	}
}
