using Infrastructure.Common.TreeList;
using RubezhAPI.GK;
using System.Linq;

namespace GKModule.ViewModels
{
	class ReportFilterDeviceViewModel : TreeNodeViewModel<ReportFilterDeviceViewModel>
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
				foreach (var child in Children)
				{
					child.IsChecked = value;
				}
				if (Parent != null)
				{
					Parent.UpdateParent();
				}
			}
		}
		void UpdateParent()
		{
			_isChecked = Children.All(x => x.IsChecked);
			OnPropertyChanged(() => IsChecked);
			if (Parent != null)
			{
				Parent.UpdateParent();
			}
		}
	}
}