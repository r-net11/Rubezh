using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class SubsystemViewModel : BaseViewModel
	{
		public SubsystemType Subsystem { get; private set; }

		public SubsystemViewModel(SubsystemType subsystem)
		{
			Subsystem = subsystem;
		}

		bool _isEnable;
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