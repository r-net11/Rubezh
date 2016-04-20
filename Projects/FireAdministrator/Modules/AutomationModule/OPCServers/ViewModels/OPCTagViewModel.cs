using RubezhAPI.Automation;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OPCTagViewModel : BaseViewModel
	{
		public OPCTag OPCTag { get; set; }
        
		public OPCTagViewModel(OPCTag opcTag)
		{
			OPCTag = opcTag;
		}

		public string Name
		{
			get { return OPCTag.Name; }
			set
			{
				OPCTag.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

        public string Uid
        {
            get { return OPCTag.Uid; }
            set
            {
                OPCTag.Uid = value;
                OnPropertyChanged(() => Uid);
                ServiceFactory.SaveService.AutomationChanged = true;
            }
        }

        public string NodeNum
        {
            get { return OPCTag.NodeNum; }
            set
            {
                OPCTag.NodeNum = value;
                OnPropertyChanged(() => NodeNum);
                ServiceFactory.SaveService.AutomationChanged = true;
            }
        }

        public string Value
        {
            get { return OPCTag.Value; }
            set
            {
                OPCTag.Value = value;
                OnPropertyChanged(() => Value);
                ServiceFactory.SaveService.AutomationChanged = true;
            }
        }

		public void Update()
		{
			OnPropertyChanged(() => OPCTag);
			OnPropertyChanged(() => NodeNum);
		    OnPropertyChanged(() => Name);
            OnPropertyChanged(() => Value);
		}
	}
}