using System;
using Infrastructure.Common.Windows.ViewModels;
using AutomationModule.Models;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class OpcDaAddingServerViewModel : BaseViewModel
	{
		public OpcDaAddingServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			Server = server;
		}

		public RubezhAPI.Automation.OpcDaServer Server { get; private set; }

		bool _isSelected;
		public bool IsSelected 
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);
			}
		}
	}
}