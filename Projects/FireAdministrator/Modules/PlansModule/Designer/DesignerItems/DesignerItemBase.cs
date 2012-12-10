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

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemBase : DesignerItem
	{
		static DesignerItemBase()
		{
			//FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
		}

		public DesignerItemBase(ElementBase element)
			: base(element)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			DeleteCommand = new RelayCommand(OnDelete);
			MouseDoubleClick += (s, e) => ShowPropertiesCommand.Execute(null);
			IsVisibleLayout = true;
			IsSelectableLayout = true;
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
					RedrawContent();
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
		protected override void CreateContextMenu()
		{
			if (ContextMenu == null)
			{
				ContextMenu = new ContextMenu();
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.CopyCommand,
					Header = "Копировать"
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).Toolbox.PlansViewModel.CutCommand,
					Header = "Вырезать",
				});
				ContextMenu.Items.Add(new Separator());
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = DeleteCommand,
					Header = "Удалить"
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ShowPropertiesCommand,
					Header = "Свойства",
				});
				ContextMenu.Items.Add(new Separator());
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveToFrontCommand,
					Header = "Вверх"
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.SendToBackCommand,
					Header = "Вниз"
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveForwardCommand,
					Header = "Выше"
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.MoveBackwardCommand,
					Header = "Ниже",
				});
				ContextMenu.Items.Add(new Separator());
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalLeftCommand,
					Header = "Выровнить по левому краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand,
					Header = "Выровнить по горизонтали",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand,
					Header = "Выровнить по правому краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand,
					Header = "Выровнить по верхнему краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand,
					Header = "Выровнить по вертикали",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand,
					Header = "Выровнить по нижнему краю",
				});
			};
		}
	}
}