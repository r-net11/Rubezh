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

namespace PlansModule.Designer.DesignerItems
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
					ServiceFactory.SaveService.PlansChanged = true;
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
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Publish(args);
			return args.PropertyViewModel as SaveCancelDialogViewModel;
		}
		public override ContextMenu GetContextMenu()
		{
			if (_contextMenu == null)
			{
				_contextMenu = new ContextMenu();

				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.CopyCommand,
					//Header = "Копировать (Ctrl+C)"
					Header = Helper.SetHeader("Копировать (Ctrl+C)", "pack://application:,,,/Controls;component/Images/BCopy.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.CutCommand,
					Header = Helper.SetHeader("Вырезать (Ctrl+X)", "pack://application:,,,/Controls;component/Images/BCut.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.PasteCommand,
					CommandParameter = DesignerCanvas,
					Header = Helper.SetHeader("Вставить (Ctrl+V)", "pack://application:,,,/Controls;component/Images/BPaste.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = DeleteCommand,
					Header = Helper.SetHeader("Удалить (Del)", "pack://application:,,,/Controls;component/Images/BDelete.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ShowPropertiesCommand,
					Header = Helper.SetHeader("Свойства", "pack://application:,,,/Controls;component/Images/BSettings.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveToFrontCommand,
					Header = Helper.SetHeader("Вверх", "pack://application:,,,/Controls;component/Images/BMoveForward.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.SendToBackCommand,
					Header = Helper.SetHeader("Вниз", "pack://application:,,,/Controls;component/Images/BMoveBackward.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveForwardCommand,
					Header = Helper.SetHeader("Выше", "pack://application:,,,/Controls;component/Images/BMoveFront.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveBackwardCommand,
					Header = Helper.SetHeader("Ниже", "pack://application:,,,/Controls;component/Images/BMoveBack.png")
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalLeftCommand,
					Header = Helper.SetHeader("Выровнять по левому краю", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-left.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand,
					Header = Helper.SetHeader("Выровнять по вертикали", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-center.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand,
					Header = Helper.SetHeader("Выровнять по правому краю", "pack://application:,,,/Controls;component/Images/bshapes-align-hori-right.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand,
					Header = Helper.SetHeader("Выровнять по верхнему краю", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-top.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand,
					Header = Helper.SetHeader("Выровнять по горизонтали", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-middle.png")
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand,
					Header = Helper.SetHeader("Выровнять по нижнему краю", "pack://application:,,,/Controls;component/Images/bshapes-align-verti-bottom.png")
				});
			};
			return _contextMenu;
		}

		public override void DragDelta(Point point, Vector shift)
		{
			base.DragDelta(point, shift);
			if (IsSelected)
				ServiceFactory.SaveService.PlansChanged = true;
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
			ServiceFactory.SaveService.PlansChanged = true;
		}
		protected override object GetToolTip()
		{
			var tooltip = Painter == null ? null : Painter.GetToolTip(Title);
			return tooltip == null ? new ImageTextTooltipViewModel() { Title = Title, ImageSource = IconSource } : tooltip;
		}
	}
}