
using System.Collections.ObjectModel;
using Controls.Menu.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
namespace PlansModule.ViewModels
{
	//public class PlansMenuViewModel : BaseViewModel
	//{
	//    public PlansMenuViewModel(PlansViewModel context)
	//    {
	//        Context = context;
	//    }

	//    public PlansViewModel Context { get; private set; }
	//}


	public class PlansMenuViewModel : MenuViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Items = new ObservableCollection<BaseViewModel>()
	        {
	            new MenuButtonViewModel(context.AddCommand, "/Controls;component/Images/Add.png" , "Добавить"),
	            new MenuButtonViewModel(context.EditCommand, "/Controls;component/Images/Edit.png" , "Редактировать"),
	            new MenuButtonViewModel(context.RemoveCommand, "/Controls;component/Images/Delete.png" , "Удалить"),
				//new MenuButtonViewModel(context.AddSubPlanCommand, "/Controls;component/Images/AddChild.png" , "Добавить дочерний план"),
				//new MenuSeparatorViewModel(),
	            new MenuButtonViewModel(context.CopyCommand, "/Controls;component/Images/Copy.png" , "Копировать"),
				new MenuButtonViewModel(context.CutCommand, "/Controls;component/Images/Cut.png" , "Вырезать"),
				new MenuButtonViewModel(context.PasteCommand, "/Controls;component/Images/Paste.png" , "Вставить"),
				new MenuButtonViewModel(context.UndoCommand, "/Controls;component/Images/Undo.png" , "Отменить"),
				new MenuButtonViewModel(context.RedoCommand, "/Controls;component/Images/Redo.png" , "Применить"),
				//new MenuSeparatorViewModel(),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveToFrontCommand, "/Controls;component/Images/MoveForward.png" , "Вверх"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.SendToBackCommand, "/Controls;component/Images/MoveBackward.png" , "Вниз"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveForwardCommand, "/Controls;component/Images/MoveFront.png" , "Выше"),
	            new MenuButtonViewModel(context.PlanDesignerViewModel.MoveBackwardCommand, "/Controls;component/Images/MoveBack.png" , "Ниже"),
				//new MenuSeparatorViewModel(),
				//context.DesignerCanvas.Toolbox
	        };
		}
	}
}
