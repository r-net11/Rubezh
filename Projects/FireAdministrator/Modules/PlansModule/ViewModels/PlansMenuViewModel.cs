using System.Collections.ObjectModel;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Plans.Common;
using Localization.Plans.ViewModels;

namespace PlansModule.ViewModels
{
	public class PlansMenuViewModel : MenuViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Items = new ObservableCollection<BaseViewModel>()
			{
				new MenuButtonViewModel(context.AddFolderCommand, "FolderOpen" , CommonViewModels.AddFolder),
				new MenuButtonViewModel(context.AddCommand, "Add" , CommonViewModels.AddPlan),
				new MenuButtonViewModel(context.EditCommand, "Edit" , CommonResources.EditPlan),
				new MenuButtonViewModel(context.RemoveCommand, "Delete" , CommonViewModels.DeletePlan),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CopyCommand, "Copy" , CommonResources.Copy),
				new MenuButtonViewModel(context.PlanDesignerViewModel.CutCommand, "Cut" , CommonResources.Cut),
				new MenuButtonViewModel(context.PlanDesignerViewModel.PasteCommand, "Paste" , CommonResources.Paste),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.UndoCommand, "Undo" , CommonViewModels.Cancel),
				new MenuButtonViewModel(context.PlanDesignerViewModel.RedoCommand, "Redo" , CommonViewModels.Apply),
				new MenuSeparatorViewModel(),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveToFrontCommand, "MoveForward" , CommonViewModels.MoveToFront),
				new MenuButtonViewModel(context.PlanDesignerViewModel.SendToBackCommand, "MoveBackward" , CommonViewModels.SendToBack),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveForwardCommand, "MoveFront" , CommonViewModels.MoveForward),
				new MenuButtonViewModel(context.PlanDesignerViewModel.MoveBackwardCommand, "MoveBack" , CommonViewModels.MoveBackward),
				new MenuSeparatorViewModel(),
				//context.DesignerCanvas.Toolbox
				
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-flip-horizontal" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-flip-vertical" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-rotate-anticlockwise" , "Ниже"),
				//new MenuButtonViewModel(context.PlanDesignerViewModel., "shapes-rotate-clockwise" , "Ниже"),
			};
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalLeftCommand, "shapes-align-hori-left", CommonViewModels.AlignHorizontalLeft));
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalCenterCommand, "shapes-align-hori-center", CommonViewModels.AlignHorizontalCenter));
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignHorizontalRightCommand, "shapes-align-hori-right", CommonViewModels.AlignHorizontalRight));
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalTopCommand, "shapes-align-verti-top", CommonViewModels.AlignVerticalTop));
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalCenterCommand, "shapes-align-verti-middle", CommonViewModels.AlignVerticalCenter));
            Items.Add(new MenuButtonViewModel(context.PlanDesignerViewModel.AlignVerticalBottomCommand, "shapes-align-verti-bottom", CommonViewModels.AlignVerticalBottom));
		}
	}
}