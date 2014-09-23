using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ExplicitTypeViewModel : BaseViewModel
	{
		public ExplicitType ExplicitType { get; private set; }

		public ExplicitTypeViewModel(ExplicitType explicitType)
		{
			ExplicitType = explicitType;
		}

		public string Name
		{
			get { return ExplicitType.ToDescription(); }
		}
	}
}
