using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcAdvosolTagGroup: OpcAdvosolElementViewModel
	{
		#region Fields And Properties

		public override string Name { get; protected set; }
		public override bool IsTag { get { return false; } }
		public override string Path { get; protected set; }
		#endregion
	}
}
