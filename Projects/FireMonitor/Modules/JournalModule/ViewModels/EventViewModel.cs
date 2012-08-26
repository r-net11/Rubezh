using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace JournalModule.ViewModels
{
	public class EventViewModel : BaseViewModel
	{
		public EventViewModel(StateType stateType, string name)
		{
			ClassId = stateType;
			Name = name;
		}

		public StateType ClassId { get; private set; }
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