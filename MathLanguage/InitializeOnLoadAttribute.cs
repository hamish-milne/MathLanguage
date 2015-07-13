using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
		AllowMultiple = false, Inherited = true)]
	public class InitializeOnLoadAttribute : Attribute
	{
	}
}
