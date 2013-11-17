using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using Controls.Menu.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ViewModels;
using Infrastructure.ViewModels;

namespace SkudModule.ViewModels
{
	public class PassCardsDesignerViewModel : MenuViewPartViewModel
	{
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PassCardDesignerViewModel PassCardDesignerViewModel { get; private set; }
		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PassCardDesignerViewModel.DesignerCanvas; }
		}

		public PassCardsDesignerViewModel()
		{
			PassCardDesignerViewModel = new PassCardDesignerViewModel();
			PassCardDesignerViewModel.IsCollapsedChanged += new EventHandler(PassCardDesignerViewModel_IsCollapsedChanged);
			OnPropertyChanged(() => PassCardDesignerViewModel);
			PassCardDesignerViewModel.DesignerCanvas.ZoomChanged();
			ElementsViewModel = new ElementsViewModel(PassCardDesignerViewModel.DesignerCanvas);

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			Menu = new MenuViewModel()
			{
				Items = new ObservableCollection<BaseViewModel>()
				{
					new MenuButtonViewModel(AddCommand, "/Controls;component/Images/Add.png" , "Добавить"),
					new MenuButtonViewModel(EditCommand, "/Controls;component/Images/Edit.png" , "Редактировать"),
					new MenuButtonViewModel(RemoveCommand, "/Controls;component/Images/Delete.png" , "Удалить"),
					//new MenuSeparatorViewModel(),
					new MenuButtonViewModel(PassCardDesignerViewModel.CopyCommand, "/Controls;component/Images/Copy.png" , "Копировать"),
					new MenuButtonViewModel(PassCardDesignerViewModel.CutCommand, "/Controls;component/Images/Cut.png" , "Вырезать"),
					new MenuButtonViewModel(PassCardDesignerViewModel.PasteCommand, "/Controls;component/Images/Paste.png" , "Вставить"),
					new MenuButtonViewModel(PassCardDesignerViewModel.UndoCommand, "/Controls;component/Images/Undo.png" , "Отменить"),
					new MenuButtonViewModel(PassCardDesignerViewModel.RedoCommand, "/Controls;component/Images/Redo.png" , "Применить"),
					//new MenuSeparatorViewModel(),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveToFrontCommand, "/Controls;component/Images/MoveForward.png" , "Вверх"),
					new MenuButtonViewModel(PassCardDesignerViewModel.SendToBackCommand, "/Controls;component/Images/MoveBackward.png" , "Вниз"),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveForwardCommand, "/Controls;component/Images/MoveFront.png" , "Выше"),
					new MenuButtonViewModel(PassCardDesignerViewModel.MoveBackwardCommand, "/Controls;component/Images/MoveBack.png" , "Ниже"),
				}
			};
		}

		public void Initialize()
		{
			using (new TimeCounter("PassCardsDesignerViewModel.Initialize: {0}"))
			{
				//using (new TimeCounter("\tPassCardsDesignerViewModel.CacheBrushes: {0}"))
				//    foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				//    {
				//        if (plan.BackgroundImageSource.HasValue && !ServiceFactory.ContentService.CheckIfExists(plan.BackgroundImageSource.Value.ToString()))
				//            plan.BackgroundImageSource = null;
				//        Helper.UpgradeBackground(plan);
				//        foreach (var elementBase in PlanEnumerator.Enumerate(plan))
				//            Helper.UpgradeBackground(elementBase);
				//    }
				PassCardTemplates = new ObservableCollection<PassCardTemplateViewModel>();
				//foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				//    AddPlan(plan, null);
				SelectedPassCardTemplate = PassCardTemplates.FirstOrDefault();
			}
		}

		private ObservableCollection<PassCardTemplateViewModel> _passCardTemplates;
		public ObservableCollection<PassCardTemplateViewModel> PassCardTemplates
		{
			get { return _passCardTemplates; }
			set
			{
				_passCardTemplates = value;
				OnPropertyChanged(() => PassCardTemplates);
			}
		}

		private PassCardTemplateViewModel _selectedPassCardTemplate;
		public PassCardTemplateViewModel SelectedPassCardTemplate
		{
			get { return _selectedPassCardTemplate; }
			set
			{
				using (new TimeCounter("PassCardsDesignerViewModel.SelectedPlan: {0}", true, true))
				{
					_selectedPassCardTemplate = value;
					OnPropertyChanged(() => SelectedPassCardTemplate);
					DesignerCanvas.Toolbox.IsEnabled = SelectedPassCardTemplate != null;
					PassCardDesignerViewModel.Save();
					PassCardDesignerViewModel.Initialize(value == null ? null : value.PassCardTemplate);
					using (new TimeCounter("\tPassCardsDesignerViewModel.UpdateElements: {0}"))
						ElementsViewModel.Update();
					DesignerCanvas.Toolbox.SetDefault();
					DesignerCanvas.DeselectAll();
				}
			}
		}

		public override void OnShow()
		{
			using (new WaitWrapper())
			using (new TimeCounter("PassCardsDesignerViewModel.OnShow: {0}"))
			{
				base.OnShow();
				DesignerCanvas.DeselectAll();
				if (DesignerCanvas.Toolbox != null)
					DesignerCanvas.Toolbox.AcceptKeyboard = true;
			}
			if (SelectedPassCardTemplate == null)
				SelectedPassCardTemplate = PassCardTemplates.FirstOrDefault();
		}
		public override void OnHide()
		{
			base.OnHide();
			if (DesignerCanvas.Toolbox != null)
				DesignerCanvas.Toolbox.AcceptKeyboard = false;
		}

		private void PassCardDesignerViewModel_IsCollapsedChanged(object sender, EventArgs e)
		{
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var dialog = new PassCardTemplatePropertiesViewModel(null);
			if (DialogService.ShowModalWindow(dialog))
				using (new WaitWrapper())
				{
					var passCardTemplateViewModel = new PassCardTemplateViewModel(dialog.PassCardTemplate);
					PassCardTemplates.Add(passCardTemplateViewModel);
					SelectedPassCardTemplate = passCardTemplateViewModel;
					// ADD TO DATABASE/MARK AS UNSAVED
					DesignerCanvas.DesignerChanged();
				}
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			string message = string.Format("Вы уверены, что хотите удалить шаблон пропуска '{0}'?", SelectedPassCardTemplate.Caption);
			if (MessageBoxService.ShowConfirmation(message) == System.Windows.MessageBoxResult.Yes)
				using (new WaitWrapper())
				{
					PassCardTemplates.Remove(SelectedPassCardTemplate);
					// DROP FROM DATABASE
					DesignerCanvas.DesignerChanged();
					SelectedPassCardTemplate = PassCardTemplates.FirstOrDefault();
				}
		}
		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var dialog = new PassCardTemplatePropertiesViewModel(SelectedPassCardTemplate.PassCardTemplate);
			if (DialogService.ShowModalWindow(dialog))
			{
				SelectedPassCardTemplate.Update();
				DesignerCanvas.Update();
				// UPDATE DATAVSE
				DesignerCanvas.Refresh();
				DesignerCanvas.DesignerChanged();
			}
		}
		private bool CanEditRemove()
		{
			return SelectedPassCardTemplate != null;
		}
	}
}