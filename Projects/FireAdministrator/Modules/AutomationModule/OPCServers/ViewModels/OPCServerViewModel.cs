using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OPCServerViewModel : BaseViewModel
	{
		public OPCServer OPCServer { get; set; }

		public OPCServerViewModel(OPCServer opcServer)
		{
			OPCServer = opcServer;
		}

		public string Name
		{
			get { return OPCServer.Name; }
			set
			{
				OPCServer.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => OPCServer);
			OnPropertyChanged(() => Name);
		}
	}
}