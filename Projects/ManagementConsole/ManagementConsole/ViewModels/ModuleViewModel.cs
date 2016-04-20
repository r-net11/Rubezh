using Infrastructure.Common.Windows.Windows.ViewModels;

namespace ManagementConsole.ViewModels
{
	public class ModuleViewModel : BaseViewModel
	{
		public ModuleViewModel(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				ManagementConsoleViewModel.Curent.HasChanges = true;
				OnPropertyChanged("IsSelected");
			}
		}
	}
}