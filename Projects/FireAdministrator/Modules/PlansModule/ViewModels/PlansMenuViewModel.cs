
using Infrastructure.Common.Windows.ViewModels;
using Controls.Menu.ViewModels;
namespace PlansModule.ViewModels
{
	public class PlansMenuViewModel : BaseViewModel
	{
		public PlansMenuViewModel(PlansViewModel context)
		{
			Context = context;
		}

		public PlansViewModel Context { get; private set; }
	}


	public class PlansMenuTestViewModel : MenuViewModel
	{
		public PlansMenuTestViewModel(PlansViewModel context)
		{
			Items = new System.Collections.ObjectModel.ObservableCollection<BaseViewModel>()
			{
				new MenuButtonViewModel(context.AddCommand, "/Controls;component/Images/Add.png" , "Добавить"),
				new MenuButtonViewModel(context.EditCommand, "/Controls;component/Images/Edit.png" , "Редактировать"),
				new MenuButtonViewModel(context.RemoveCommand, "/Controls;component/Images/Delete.png" , "Удалить"),
				new MenuButtonViewModel(context.AddSubPlanCommand, "/Controls;component/Images/AddChild.png" , "Добавить дочерний план"),
			};
		}
	}
}
