using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ParameterTemplatesMenuViewModel : BaseViewModel
	{
		public ParameterTemplatesMenuViewModel(ParameterTemplatesViewModel context)
		{
			Context = context;
		}

		public ParameterTemplatesViewModel Context { get; private set; }
	}
}