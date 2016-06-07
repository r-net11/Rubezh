using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Automation;

namespace SoundsModule.ViewModels
{
	public class SoundFileDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSound Sound { get; private set; }

		public SoundFileDetailsViewModel(AutomationSound sound)
		{
			Title = "Свойства звукового элемента";
			Sound = sound;
			Name = Sound.Name;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Sound.Name = Name;
			return base.Save();
		}
	}
}