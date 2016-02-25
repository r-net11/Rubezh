using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.Automation;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class OpcTagFilterViewModel : BaseViewModel
	{
		public OpcTagFilterViewModel(OpcDaTagFilter filter)
		{
			OpcTagFilter = filter;
		}

		public OpcDaTagFilter OpcTagFilter { get; set; }

		public string Name
		{
			get { return OpcTagFilter.Name; }
			set
			{
				OpcTagFilter.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}
