using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	public class StateTypeName<TStateType>
	{
		public TStateType StateType { get; set; }
		public string Name { get; set; }
	}
}