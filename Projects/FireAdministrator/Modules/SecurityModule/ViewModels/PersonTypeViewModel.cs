using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class PersonTypeViewModel : BaseViewModel
	{
		public PersonType PersonType { get; private set; }
		public string Name { get; private set; }

		public PersonTypeViewModel(PersonType personType)
		{
			PersonType = personType;
			Name = PersonType.ToDescription();
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
