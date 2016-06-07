using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Automation;

namespace SoundsModule.ViewModels
{
	public class SoundFileViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public AutomationSound Sound { get; set; }

		public SoundFileViewModel(AutomationSound sound)
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