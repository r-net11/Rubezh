using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using System;
using System.Linq;
using System.Windows;

namespace Infrastructure.Designer.ViewModels
{
	public partial class BasePlanDesignerViewModel : BaseViewModel
	{
		void InitializeAlignCommands()
		{
			AlignHorizontalLeftCommand = new RelayCommand(OnAlignHorizontalLeftCommand, CanAlignExecute);
			AlignHorizontalCenterCommand = new RelayCommand(OnAlignHorizontalCenterCommand, CanAlignExecute);
			AlignHorizontalRightCommand = new RelayCommand(OnAlignHorizontalRightCommand, CanAlignExecute);
			AlignVerticalTopCommand = new RelayCommand(OnAlignVerticalTopCommand, CanAlignExecute);
			AlignVerticalCenterCommand = new RelayCommand(OnAlignVerticalCenterCommand, CanAlignExecute);
			AlignVerticalBottomCommand = new RelayCommand(OnAlignVerticalBottomCommand, CanAlignExecute);
		}

		public bool CanAlignExecute(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 1;
		}

		public RelayCommand AlignHorizontalLeftCommand { get; private set; }
		private void OnAlignHorizontalLeftCommand()
		{
			Align((item, root) =>
				{
					Rect rect = item.Element.GetRectangle();
					item.Element.SetPosition(new Point(root.Left + rect.Width / 2, rect.Top + rect.Height / 2));
				});
		}
		public RelayCommand AlignHorizontalCenterCommand { get; private set; }
		private void OnAlignHorizontalCenterCommand()
		{
			Align((item, root) =>
			{
				Rect rect = item.Element.GetRectangle();
				item.Element.SetPosition(new Point(root.Left + root.Width / 2, rect.Top + rect.Height / 2));
			});
		}
		public RelayCommand AlignHorizontalRightCommand { get; private set; }
		private void OnAlignHorizontalRightCommand()
		{
			Align((item, root) =>
			{
				Rect rect = item.Element.GetRectangle();
				item.Element.SetPosition(new Point(root.Right - rect.Width / 2, rect.Top + rect.Height / 2));
			});
		}
		public RelayCommand AlignVerticalTopCommand { get; private set; }
		private void OnAlignVerticalTopCommand()
		{
			Align((item, root) =>
			{
				Rect rect = item.Element.GetRectangle();
				item.Element.SetPosition(new Point(rect.Left + rect.Width / 2, root.Top + rect.Height / 2));
			});
		}
		public RelayCommand AlignVerticalCenterCommand { get; private set; }
		private void OnAlignVerticalCenterCommand()
		{
			Align((item, root) =>
			{
				Rect rect = item.Element.GetRectangle();
				item.Element.SetPosition(new Point(rect.Left + rect.Width / 2, root.Top + root.Height / 2));
			});
		}
		public RelayCommand AlignVerticalBottomCommand { get; private set; }
		private void OnAlignVerticalBottomCommand()
		{
			Align((item, root) =>
			{
				Rect rect = item.Element.GetRectangle();
				item.Element.SetPosition(new Point(rect.Left + rect.Width / 2, root.Bottom - rect.Height / 2));
			});
		}

		private void Align(Action<DesignerItem, Rect> transform)
		{
			Rect total = Rect.Empty;
			foreach (var designerItem in DesignerCanvas.SelectedItems)
				total.Union(designerItem.Element.GetRectangle());
			DesignerCanvas.BeginChange();
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				transform(designerItem, total);
				//designerItem.Translate();
				designerItem.RefreshPainter();
			}
			DesignerCanvas.EndChange();
			DesignerCanvas.DesignerChanged();
		}
	}
}
