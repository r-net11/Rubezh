using StrazhAPI.Automation;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public AutomationSound Sound { get; set; }

		public SoundViewModel(AutomationSound sound)
		{
			Sound = sound;
		}

		public string Name
		{
			get { return Sound.Name; }
			set
			{
				Sound.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Sound);
			OnPropertyChanged(() => Name);
		}
	}
}