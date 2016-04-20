using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.Automation;
using Infrastructure;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class OpcTagFilterViewModel : BaseViewModel
	{
		public OpcTagFilterViewModel(OpcDaTagFilter filter)
		{
			OpcDaTagFilter = filter;

			foreach(var server in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers)
			{
				var tag = server.Tags.FirstOrDefault(t => t.Uid == OpcDaTagFilter.TagUID);
				if (tag != null)
				{
					OpcDaTag = tag;
					OpcDaServer = server;
					break;
				}
			}
		}

		public OpcDaTagFilter OpcDaTagFilter { get; private set; }

		OpcDaTag _opcDaTag;
		public OpcDaTag OpcDaTag 
		{
			get { return _opcDaTag; }
			set
			{
				_opcDaTag = value;
				OnPropertyChanged(() => OpcDaTag);
			}
		}

		OpcDaServer _opcDaServer;
		public OpcDaServer OpcDaServer 
		{
			get { return _opcDaServer; }
			set
			{
				_opcDaServer = value;
				OnPropertyChanged(() => OpcDaServer);
			}
		}

		public string Name
		{
			get { return OpcDaTagFilter.Name; }
			set
			{
				OpcDaTagFilter.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public string Description
		{
			get { return OpcDaTagFilter.Description; }
			set
			{
				OpcDaTagFilter.Description = value;
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public double Hysteresis
		{
			get { return OpcDaTagFilter.Hysteresis; }
			set
			{
				OpcDaTagFilter.Hysteresis = value;
				OnPropertyChanged(() => Hysteresis);
			}
		}
	}
}