using System.Windows;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using System.Windows.Media.Imaging;
using System;
using Infrastructure.Client.Plans.ViewModels;
using Infrastructure.Common.Services;

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

				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.CopyCommand,
					//Header = "Копировать (Ctrl+C)"
					Header = DesignerCanvasHelper.BuildMenuHeader("Копировать (Ctrl+C)", "pack://application:,,,/Controls;component/Images/BCopy.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.CutCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Вырезать (Ctrl+X)", "pack://application:,,,/Controls;component/Images/BCut.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.PasteCommand,
					CommandParameter = DesignerCanvas,
					Header = DesignerCanvasHelper.BuildMenuHeader("Вставить (Ctrl+V)", "pack://application:,,,/Controls;component/Images/BPaste.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = DeleteCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Удалить (Del)", "pack://application:,,,/Controls;component/Images/BDelete.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ShowPropertiesCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Свойства", "pack://application:,,,/Controls;component/Images/BSettings.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveToFrontCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Вверх", "pack://application:,,,/Controls;component/Images/BMoveForward.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.SendToBackCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Вниз", "pack://application:,,,/Controls;component/Images/BMoveBackward.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveForwardCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выше", "pack://application:,,,/Controls;component/Images/BMoveFront.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveBackwardCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Ниже", "pack://application:,,,/Controls;component/Images/BMoveBack.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalLeftCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по левому краю", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-left.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по вертикали", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-center.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по правому краю", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-right.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по верхнему краю", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-top.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по горизонтали", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-middle.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand,
					Header = DesignerCanvasHelper.BuildMenuHeader("Выровнять по нижнему краю", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-bottom.png")
				});
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