using System.Windows;
using System.Windows.Controls;
using Infrastructure.Client.Plans.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace Infrastructure.Designer.DesignerItems
{
	public class DesignerItemBase : DesignerItem
	{
		private ContextMenu _contextMenu;
		public DesignerItemBase(ElementBase element)
			: base(element)
		{
			_contextMenu = null;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			DeleteCommand = new RelayCommand(OnDelete);
		}

		protected override void OnShowProperties()
		{
			var property = CreatePropertiesViewModel();
			if (property != null)
			{
				DesignerCanvas.BeginChange();
				if (((DesignerCanvas)DesignerCanvas).Toolbox != null)
					((DesignerCanvas)DesignerCanvas).Toolbox.AcceptKeyboard = false;
				if (DialogService.ShowModalWindow(property))
				{
					OnDesignerItemPropertyChanged();
					Painter.Invalidate();
					if (ResizeChrome != null)
						ResizeChrome.InvalidateVisual();
					DesignerCanvas.Refresh();
					DesignerCanvas.DesignerChanged();
					DesignerCanvas.EndChange();
				}
				if (((DesignerCanvas)DesignerCanvas).Toolbox != null)
					((DesignerCanvas)DesignerCanvas).Toolbox.AcceptKeyboard = true;
			}
		}
		protected override void OnDelete()
		{
			((DesignerCanvas)DesignerCanvas).RemoveAllSelected();
		}
		protected virtual SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			var args = new ShowPropertiesEventArgs(Element);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Publish(args);
			return args.PropertyViewModel as SaveCancelDialogViewModel;
		}
		public override ContextMenu GetContextMenu()
		{
			if (_contextMenu == null)
			{
				_contextMenu = new ContextMenu();

				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Копировать (Ctrl+C)", 
					"pack://application:,,,/Controls;component/Images/BCopy.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.CopyCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Вырезать (Ctrl+X)", 
					"pack://application:,,,/Controls;component/Images/BCut.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.CutCommand)
				);
				
				var menuItem = DesignerCanvasHelper.BuildMenuItem(
					"Вставить (Ctrl+V)", 
					"pack://application:,,,/Controls;component/Images/BPaste.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.PasteCommand
				);
				menuItem.CommandParameter = DesignerCanvas;
				_contextMenu.Items.Add(menuItem);
				
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Удалить (Del)", 
					"pack://application:,,,/Controls;component/Images/BDelete.png", 
					DeleteCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Свойства", 
					"pack://application:,,,/Controls;component/Images/BSettings.png", 
					ShowPropertiesCommand
				));
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Вверх", 
					"pack://application:,,,/Controls;component/Images/BMoveForward.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveToFrontCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Вниз", 
					"pack://application:,,,/Controls;component/Images/BMoveBackward.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.SendToBackCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выше", 
					"pack://application:,,,/Controls;component/Images/BMoveFront.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveForwardCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Ниже", 
					"pack://application:,,,/Controls;component/Images/BMoveBack.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveBackwardCommand
				));
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по левому краю", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-hori-left.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalLeftCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по вертикали", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-hori-center.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по правому краю", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-hori-right.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по верхнему краю", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-verti-top.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по горизонтали", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-verti-middle.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand
				));
				_contextMenu.Items.Add(DesignerCanvasHelper.BuildMenuItem(
					"Выровнять по нижнему краю", 
					"pack://application:,,,/Controls;component/Images/bshapes-align-verti-bottom.png", 
					((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand
				));
			};
			return _contextMenu;
		}

		public override void DragDelta(Point point, Vector shift)
		{
			base.DragDelta(point, shift);
			if (IsSelected)
				DesignerCanvas.DesignerChanged();
		}

		//public override void DragStarted(Point point)
		//{
		//    if (IsSelected)
		//    {
		//        IsBusy = true;
		//        ((DesignerCanvas)DesignerCanvas).BeginMove(point);
		//    }
		//}
		//public override void DragCompleted(Point point)
		//{
		//    IsBusy = false;
		//    ((DesignerCanvas)DesignerCanvas).EndMove();
		//}

		protected override void OnChanged()
		{
			base.OnChanged();
			DesignerCanvas.DesignerChanged();
		}
		protected override object GetToolTip()
		{
			var tooltip = Painter == null ? null : Painter.GetToolTip(Title);
			return tooltip == null ? new ImageTextTooltipViewModel() { Title = Title, ImageSource = IconSource } : tooltip;
		}
	}
}