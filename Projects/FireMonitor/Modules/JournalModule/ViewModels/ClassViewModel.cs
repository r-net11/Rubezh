using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ClassViewModel : BaseViewModel
	{
		public ClassViewModel(StateType stateType)
		{
			StateType = stateType;
		}

		public StateType StateType { get; private set; }

		bool? _isEnable = false;
		public bool? IsEnable
		{
			get { return _isEnable; }
			set
			{
				_isEnable = value;
				OnPropertyChanged("IsEnable");
			}
		}
	}
}