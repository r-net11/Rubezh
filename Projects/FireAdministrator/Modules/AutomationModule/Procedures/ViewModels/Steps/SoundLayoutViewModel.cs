using FiresecAPI.Automation;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class SoundLayoutViewModel : BaseViewModel
	{
		public LayoutModel Layout { get; private set; }
		public SoundArguments SoundArguments { get; private set; }

		public SoundLayoutViewModel(SoundArguments soundArguments, LayoutModel layout)
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
				if (value && !SoundArguments.LayoutsUids.Contains(Layout.UID))
					SoundArguments.LayoutsUids.Add(Layout.UID);
				else if (!value)
					SoundArguments.LayoutsUids.Remove(Layout.UID);
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}