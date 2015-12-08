using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcAdvosolTagViewModel: OpcAdvosolElementViewModel
	{
		#region Constructors

		#endregion

		#region Fields And Properties
		
		public override string Name { get; protected set; }
		public override bool IsTag { get { return true; } }
		public override string Path { get; protected set; }
		public string TagId { get; private set; }

		#endregion
	}
}
