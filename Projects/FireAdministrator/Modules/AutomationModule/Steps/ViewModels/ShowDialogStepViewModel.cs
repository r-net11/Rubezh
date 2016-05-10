using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ShowDialogStepViewModel : BaseStepViewModel
	{
		public ShowDialogArguments ShowDialogArguments { get; private set; }

		public ShowDialogStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowDialogArguments = stepViewModel.Step.ShowDialogArguments;
		}

		public bool ForAllClients
		{
			get { return ShowDialogArguments.ForAllClients; }
			set
			{
				ShowDialogArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
		public bool IsModalWindow
		{
			get { return ShowDialogArguments.IsModalWindow; }
			set
			{
				ShowDialogArguments.IsModalWindow = value;
				OnPropertyChanged(() => IsModalWindow);
			}
		}
		public string Title
		{
			get { return ShowDialogArguments.Title; }
			set
			{
				ShowDialogArguments.Title = value;
				OnPropertyChanged(() => Title);
			}
		}

		public bool AllowClose
		{
			get { return ShowDialogArguments.AllowClose; }
			set
			{
				ShowDialogArguments.AllowClose = value;
				OnPropertyChanged(() => AllowClose);
			}
		}
		public bool AllowMaximize
		{
			get { return ShowDialogArguments.AllowMaximize; }
			set
			{
				ShowDialogArguments.AllowMaximize = value;
				OnPropertyChanged(() => AllowMaximize);
			}
		}
		public bool Sizable
		{
			get { return ShowDialogArguments.Sizable; }
			set
			{
				ShowDialogArguments.Sizable = value;
				OnPropertyChanged(() => Sizable);
			}
		}
		public bool TopMost
		{
			get { return ShowDialogArguments.TopMost; }
			set
			{
				ShowDialogArguments.TopMost = value;
				OnPropertyChanged(() => TopMost);
			}
		}
		public double Width
		{
			get { return ShowDialogArguments.Width; }
			set
			{
				ShowDialogArguments.Width = value;
				OnPropertyChanged(() => Width);
			}
		}
		public double Height
		{
			get { return ShowDialogArguments.Height; }
			set
			{
				ShowDialogArguments.Height = value;
				OnPropertyChanged(() => Height);
			}
		}
		public double MinWidth
		{
			get { return ShowDialogArguments.MinWidth; }
			set
			{
				ShowDialogArguments.MinWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}
		public double MinHeight
		{
			get { return ShowDialogArguments.MinHeight; }
			set
			{
				ShowDialogArguments.MinHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}

		private ProcedureLayoutCollectionViewModel _procedureLayoutCollectionViewModel;
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel
		{
			get { return _procedureLayoutCollectionViewModel; }
			private set
			{
				_procedureLayoutCollectionViewModel = value;
				OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
			}
		}

		public ObservableCollection<LayoutViewModel> Layouts { get; private set; }
		private LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				_selectedLayout = value;
				ShowDialogArguments.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override string Description
		{
			get
			{
				return string.Format(StepCommonViewModel.ShowDialog, SelectedLayout == null ? ArgumentViewModel.EmptyText : SelectedLayout.Name, IsModalWindow ? StepCommonViewModel.ShowDialog_Modal : StepCommonViewModel.ShowDialog_NotModal);
			}
		}
		public override void UpdateContent()
		{
			Layouts = new ObservableCollection<LayoutViewModel>(FiresecManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ShowDialogArguments.Layout);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowDialogArguments.LayoutFilter);
		}
	}
}
