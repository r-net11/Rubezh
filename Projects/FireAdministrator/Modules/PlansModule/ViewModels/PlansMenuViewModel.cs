using System.Collections.ObjectModel;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class PlansMenuViewModel : MenuViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Items = new ObservableCollection<BaseViewModel>()
			{
				new MenuButtonViewModel(context.AddFolderCommand, "FolderOpen" , "Добавить папку"),
				new MenuButtonViewModel(context.AddCommand, "Add" , "Добавить план"),
				new MenuButtonViewModel(context.EditCommand, "Edit" , "Редактировать план"),
				new MenuButtonViewModel(context.RemoveCommand, "Delete" , "Удалить план"),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CopyCommand, "Copy" , "Копировать"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CutCommand, "Cut" , "Вырезать"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.PasteCommand, "Paste" , "Вставить"),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.UndoCommand, "Undo" , "Отменить"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.RedoCommand, "Redo" , "Применить"),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveToFrontCommand, "MoveForward" , "Вверх"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.SendToBackCommand, "MoveBackward" , "Вниз"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveForwardCommand, "MoveFront" , "Выше"),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveBackwardCommand, "MoveBack" , "Ниже"),
				new MenuSeparatorViewModel(),
				//context.DesignerCanvas.Toolbox
				
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-flip-horizontal" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-flip-vertical" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-rotate-anticlockwise" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-rotate-clockwise" , "Ниже"),
			};
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalLeftCommand, "shapes-align-hori-left", "Выровнять по левому краю"));
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalCenterCommand, "shapes-align-hori-center", "Выровнять по вертикали"));
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalRightCommand, "shapes-align-hori-right", "Выровнять по правому краю"));
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalTopCommand, "shapes-align-verti-top", "Выровнять по верхнему краю"));
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalCenterCommand, "shapes-align-verti-middle", "Выровнять по горизонтали"));
			Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalBottomCommand, "shapes-align-verti-bottom", "Выровнять по нижнему краю"));
		}
	}
}