using System;
using System.Collections.Generic;
using System.Text;

namespace MathLanguage
{
	public interface ITypeMask
	{
		Value TrySetValue(Value newValue);
	}
}
