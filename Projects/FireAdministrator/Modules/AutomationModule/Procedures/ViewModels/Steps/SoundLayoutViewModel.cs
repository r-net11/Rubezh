using FiresecAPI.Automation;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundLayoutViewModel : BaseViewModel
	{
		public Layout Layout { get; private set; }
		public SoundArguments SoundArguments { get; private set; }

		public SoundLayoutViewModel(SoundArguments soundArguments, Layout layout)
		{
			Layout = layout;
			SoundArguments = soundArguments;
		}

		public string Name
		{
			get { return Layout.Caption; }
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				if (value)
					SoundArguments.LayoutsUids.Add(Layout.UID);
				else
					SoundArguments.LayoutsUids.Remove(Layout.UID);
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsChecked);
			}
		}

	}
}
