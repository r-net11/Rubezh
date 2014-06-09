using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class MaskViewModel : BaseViewModel
	{
		public Mask Mask { get; set; }

		public MaskViewModel(Mask mask)
		{
			Mask = mask;
		}

		public string Name
		{
			get { return Mask.Name; }
			set
			{
				Mask.Name = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(Mask mask)
		{
			Mask = mask;
			OnPropertyChanged("Name");
		}
	}
}
