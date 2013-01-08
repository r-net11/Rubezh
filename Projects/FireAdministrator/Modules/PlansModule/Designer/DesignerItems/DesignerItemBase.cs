using System.Windows;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

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
			MouseLeftButtonDown += (s, e) =>
				{
					if (e.ClickCount == 2)
					{
						ShowPropertiesCommand.Execute(null);
						e.Handled = true;
					}
				};
			//MouseDoubleClick += (s, e) => ShowPropertiesCommand.Execute(null);
			IsVisibleLayout = true;
			IsSelectableLayout = true;
			//IsSelected = true;
		}

		public override void UpdateAdornerLayout()
		{
			LoadTemplate();
			base.UpdateAdornerLayout();
		}
		private void LoadTemplate()
		{
			var grid = new Grid();
			//grid.SetBinding(ToolTipProperty, new Binding("Title"));
			//grid.SetBinding(IsHitTestVisibleProperty, new Binding("IsSelectable"));
			grid.DataContext = this;
			//var decorator = new ResizeDecorator2();
			//decorator.SetBinding(ResizeDecorator2.ShowDecoratorProperty, new Binding("IsSelected"));
			//decorator.SetBinding(ResizeDecorator2.VisibilityProperty, new Binding("IsSelectable") { Converter = new BooleanToVisibilityConverter() });
			//grid.Children.Add(decorator);
			if (ResizeChrome != null)
			{
				//ResizeChrome.SetBinding(ResizeChrome.VisibilityProperty, new Binding("IsSelected") { Converter = new BooleanToVisibilityConverter() });
				grid.Children.Add(ResizeChrome);
			}
			grid.Children.Add(new MoveThumb());
			Content = grid;
			IsSelectableChanged();
			IsSelectedChanged();
			TitleChanged();
			//UpdateLayout();
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
					Header = "Выровнять по левому краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalCenterCommand,
					Header = "Выровнять по вертикали",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignHorizontalRightCommand,
					Header = "Выровнять по правому краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalTopCommand,
					Header = "Выровнять по верхнему краю",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalCenterCommand,
					Header = "Выровнять по горизонтали",
				});
				ContextMenu.Items.Add(new MenuItem()
				{
					Command = ((DesignerCanvas)DesignerCanvas).PlanDesignerViewModel.AlignVerticalBottomCommand,
					Header = "Выровнять по нижнему краю",
				});
			};
		}

		protected override void IsSelectableChanged()
		{
			base.IsSelectableChanged();
			if (Content != null)
				((FrameworkElement)Content).IsHitTestVisible = IsSelectable;
		}
		protected override void IsSelectedChanged()
		{
			base.IsSelectedChanged();
			if (ResizeChrome != null)
				ResizeChrome.Visibility = IsSelected ? Visibility.Visible : Visibility.Hidden;
		}
		protected override void TitleChanged()
		{
			base.TitleChanged();
			if (Content != null)
				((FrameworkElement)Content).ToolTip = Title;
		}
	}
}