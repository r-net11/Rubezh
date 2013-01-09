using System.Windows;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;

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

		public override void UpdateAdornerLayout()
		{
			LoadTemplate();
			base.UpdateAdornerLayout();
		}
		private void LoadTemplate()
		{
			//var grid = new Grid();
			//grid.DataContext = this;
			//grid.Children.Add(new MoveThumb());
			//if (ResizeChrome != null)
			//    grid.Children.Add(ResizeChrome);
			//Content = grid;
			//IsSelectableChanged();
			//IsSelectedChanged();
			//TitleChanged();
		}

		protected override void OnShowProperties()
		{
			var property = CreatePropertiesViewModel();
			if (property != null)
			{
				DesignerCanvas.BeginChange();
				if (DialogService.ShowModalWindow(property))
				{
					OnDesignerItemPropertyChanged();
					Redraw();
					ServiceFactory.SaveService.PlansChanged = true;
					DesignerCanvas.EndChange();
				}
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
					Header = "Копировать"
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.CutCommand,
					Header = "Вырезать",
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = DeleteCommand,
					Header = "Удалить"
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ShowPropertiesCommand,
					Header = "Свойства",
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveToFrontCommand,
					Header = "Вверх"
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.SendToBackCommand,
					Header = "Вниз"
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveForwardCommand,
					Header = "Выше"
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveBackwardCommand,
					Header = "Ниже",
				});
				_contextMenu.Items.Add(new Separator());
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalLeftCommand,
					Header = "Выровнять по левому краю",
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand,
					Header = "Выровнять по вертикали",
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand,
					Header = "Выровнять по правому краю",
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand,
					Header = "Выровнять по верхнему краю",
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand,
					Header = "Выровнять по горизонтали",
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand,
					Header = "Выровнять по нижнему краю",
				});
			};
			return _contextMenu;
		}

		protected override void DragDelta(Vector shift)
		{
			base.DragDelta(shift);
			if (IsSelected)
				ServiceFactory.SaveService.PlansChanged = true;
		}
	}
}