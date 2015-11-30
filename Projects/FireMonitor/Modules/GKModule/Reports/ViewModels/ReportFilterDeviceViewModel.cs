using Infrastructure.Common.TreeList;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	class ReportFilterDeviceViewModel: TreeNodeViewModel<ReportFilterDeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		
		public ReportFilterDeviceViewModel(GKDevice device)
		{
			Device = device;
		}
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