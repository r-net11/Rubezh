using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Sounds.Common;
using StrazhAPI.Automation;
using StrazhAPI.Models;

namespace SoundsModule.ViewModels
{
	public class SoundFileViewModel : BaseViewModel
	{
		public string DefaultName = CommonResources.None;


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

		public SoundLibraryType SoundLibraryType
		{
			get { return Sound.SoundLibraryType; }
			set
			{
				Sound.SoundLibraryType = value;
				OnPropertyChanged(() => SoundLibraryType);
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