using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class EventViewModel : BaseViewModel
	{
		public EventViewModel(StateType stateType, string name)
		{
			StateType = stateType;
			Name = name;
		}

		public StateType StateType { get; private set; }
		public string Name { get; private set; }

		bool _isEnable = false;
		public bool IsEnable
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