using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using System.Windows.Controls;

namespace Infrastructure.Plans.ViewModels
{
	public class ElementBaseViewModel : TreeNodeViewModel<ElementBaseViewModel>
	{
		public RelayCommand ShowOnPlanCommand { get; protected set; }

		public virtual ContextMenu ContextMenu
		{
			get { return null; }
		}

		public virtual object ToolTip
		{
			get { return null; }
		}

		private bool _isBold;
		public bool IsBold
		{
			get { return _isBold; }
			set
			{
				_isBold = value;
				OnPropertyChanged(() => IsBold);
			}
		}
		private bool _isGroupHaveChild;
		public bool IsGroupHasChild
		{
			get { return _isGroupHaveChild; }
			set
			{
				_isGroupHaveChild = value;
				OnPropertyChanged(() => IsGroupHasChild);
			}
		}
		public string IconSource { get; protected set; }
	}
}
