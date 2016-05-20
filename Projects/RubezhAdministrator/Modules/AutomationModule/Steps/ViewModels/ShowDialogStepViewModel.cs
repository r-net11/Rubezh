using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ShowDialogStepViewModel : BaseStepViewModel
	{
		public ShowDialogStep ShowDialogStep { get; private set; }
		public ArgumentViewModel WindowUIDArgument { get; private set; }

		public ShowDialogStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowDialogStep = (ShowDialogStep)stepViewModel.Step;
			IsServerContext = Procedure.ContextType == ContextType.Server;
			WindowUIDArgument = new ArgumentViewModel(ShowDialogStep.WindowIDArgument, stepViewModel.Update, UpdateContent);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowDialogStep.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public bool ForAllClients
		{
			get { return ShowDialogStep.ForAllClients; }
			set
			{
				ShowDialogStep.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
		public bool IsModalWindow
		{
			get { return ShowDialogStep.IsModalWindow; }
			set
			{
				ShowDialogStep.IsModalWindow = value;
				OnPropertyChanged(() => IsModalWindow);
			}
		}
		public string Title
		{
			get { return ShowDialogStep.Title; }
			set
			{
				ShowDialogStep.Title = value;
				OnPropertyChanged(() => Title);
			}
		}

		public bool AllowClose
		{
			get { return ShowDialogStep.AllowClose; }
			set
			{
				ShowDialogStep.AllowClose = value;
				OnPropertyChanged(() => AllowClose);
			}
		}
		public bool AllowMaximize
		{
			get { return ShowDialogStep.AllowMaximize; }
			set
			{
				ShowDialogStep.AllowMaximize = value;
				OnPropertyChanged(() => AllowMaximize);
			}
		}
		public bool Sizable
		{
			get { return ShowDialogStep.Sizable; }
			set
			{
				ShowDialogStep.Sizable = value;
				OnPropertyChanged(() => Sizable);
			}
		}
		public bool TopMost
		{
			get { return ShowDialogStep.TopMost; }
			set
			{
				ShowDialogStep.TopMost = value;
				OnPropertyChanged(() => TopMost);
			}
		}
		public double Width
		{
			get { return ShowDialogStep.Width; }
			set
			{
				ShowDialogStep.Width = value;
				OnPropertyChanged(() => Width);
			}
		}
		public double Height
		{
			get { return ShowDialogStep.Height; }
			set
			{
				ShowDialogStep.Height = value;
				OnPropertyChanged(() => Height);
			}
		}
		public double MinWidth
		{
			get { return ShowDialogStep.MinWidth; }
			set
			{
				ShowDialogStep.MinWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}
		public double MinHeight
		{
			get { return ShowDialogStep.MinHeight; }
			set
			{
				ShowDialogStep.MinHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}

		public bool CustomPosition
		{
			get { return ShowDialogStep.CustomPosition; }
			set
			{
				ShowDialogStep.CustomPosition = value;
				OnPropertyChanged(() => CustomPosition);
			}
		}
		public double Left
		{
			get { return ShowDialogStep.Left; }
			set
			{
				ShowDialogStep.Left = value;
				OnPropertyChanged(() => Left);
			}
		}
		public double Top
		{
			get { return ShowDialogStep.Top; }
			set
			{
				ShowDialogStep.Top = value;
				OnPropertyChanged(() => Top);
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
				ShowDialogStep.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override string Description
		{
			get
			{
				return string.Format("Открыть диалог: {0} {1}; ID={2}", SelectedLayout == null ? ArgumentViewModel.EmptyText : SelectedLayout.Name, IsModalWindow ? "(модальный)" : "(не модальный)", WindowUIDArgument.Description);
			}
		}

		bool _isServerContext;
		public bool IsServerContext
		{
			get { return _isServerContext; }
			set
			{
				_isServerContext = value;
				OnPropertyChanged(() => IsServerContext);
			}
		}
		public override void UpdateContent()
		{
			Layouts = new ObservableCollection<LayoutViewModel>(ClientManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ShowDialogStep.Layout);
			WindowUIDArgument.Update(Procedure, ExplicitType.String);
		}
	}
}
