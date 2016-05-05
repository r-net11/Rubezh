using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentFilterItemViewModel : OrganisationElementViewModel<DepartmentFilterItemViewModel, ShortDepartment>
	{
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