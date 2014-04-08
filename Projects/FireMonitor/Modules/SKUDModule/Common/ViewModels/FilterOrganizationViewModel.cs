using FiresecAPI;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class FilterOrganizationViewModel : CheckBoxItemViewModel
	{
		public FilterOrganizationViewModel(Organization organization)
		{
			Name = organization.Name;
			Organization = organization;
		}

		public string Name { get; private set; }
		public Organization Organization { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}