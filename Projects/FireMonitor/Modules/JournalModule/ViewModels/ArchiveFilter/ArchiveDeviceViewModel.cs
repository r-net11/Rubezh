using RubezhAPI.GK;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.TreeList;

namespace JournalModule.ViewModels
{
	public class ArchiveDeviceViewModel : TreeNodeViewModel<ArchiveDeviceViewModel>, ICheckBoxItem
	{
		public ArchiveDeviceViewModel(GKDevice device)
		{
			Device = device;
			Name = device.PresentationName;
		}

		public GKDevice Device { get; private set; }
		public string Name { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (ItemsList != null)
					ItemsList.Update();
			}
		}

		public void SetFromParent(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}

		public ICheckBoxItemList ItemsList { get; set; }

		public bool CanCheck
		{
			get { return Device.IsRealDevice; }
		}
	}
}