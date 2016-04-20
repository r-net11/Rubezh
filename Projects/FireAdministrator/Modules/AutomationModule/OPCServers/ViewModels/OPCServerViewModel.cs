using RubezhAPI.Automation;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;

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

        public string Address
        {
            get { return OPCServer.Address; }
            set
            {
                OPCServer.Address = value;
                OnPropertyChanged(() => Address);
                ServiceFactory.SaveService.AutomationChanged = true;
            }
        }

		public void Update()
		{
			OnPropertyChanged(() => OPCServer);
			OnPropertyChanged(() => Name);
		    OnPropertyChanged(() => Address);
		}
	}
}