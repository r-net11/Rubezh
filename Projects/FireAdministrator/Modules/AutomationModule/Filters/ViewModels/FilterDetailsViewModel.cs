using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationFilter Filter { get; private set; }
		public FilterDetailsViewModel(AutomationFilter filter)
		{
			Title = "Свойства фильтра";
			Filter = filter;
			Name = filter.Name;
		}

		public FilterDetailsViewModel()
		{
			Title = "Добавить фильтр";
			Filter = new AutomationFilter();
			Name = Filter.Name;
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
			Filter.Name = Name;
			return base.Save();
		}
	}
}