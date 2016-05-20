using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ModuleViewModel : BaseViewModel
	{
		public ModuleViewModel(string name, string description, bool canChange = true)
		{
			Name = name;
			Description = description;
			CanChange = canChange;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool CanChange { get; private set; }

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