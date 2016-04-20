using System.Collections.ObjectModel;
using Controls.Menu.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class PlansMenuViewModel : MenuViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Items = new ObservableCollection<BaseViewModel>()
			{
				//new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CopyCommand, "Copy", "Копировать"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CutCommand, "Cut", "Вырезать"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.PasteCommand, "Paste", "Вставить"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.UndoCommand, "Undo", "Отменить"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.RedoCommand, "Redo", "Применить"),
				//new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveToFrontCommand, "MoveForward", "Вверх"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.SendToBackCommand, "MoveBackward", "Вниз"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveForwardCommand, "MoveFront", "Выше"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveBackwardCommand, "MoveBack", "Ниже"),
				//new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalLeftCommand, "shapes-align-hori-left", "Выровнять по левому краю"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalCenterCommand, "shapes-align-hori-center", "Выровнять по вертикали"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalRightCommand, "shapes-align-hori-right", "Выровнять по правому краю"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalTopCommand, "shapes-align-verti-top", "Выровнять по верхнему краю"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalCenterCommand, "shapes-align-verti-middle", "Выровнять по горизонтали"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalBottomCommand, "shapes-align-verti-bottom", "Выровнять по нижнему краю"),
			};
		}
	}
}