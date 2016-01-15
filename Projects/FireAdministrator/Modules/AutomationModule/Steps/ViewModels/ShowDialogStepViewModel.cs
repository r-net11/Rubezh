﻿using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ShowDialogStepViewModel : BaseStepViewModel
	{
		public ShowDialogArguments ShowDialogArguments { get; private set; }
		public ArgumentViewModel WindowUIDArgument { get; private set; }

		public ShowDialogStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowDialogArguments = stepViewModel.Step.ShowDialogArguments;
			IsServerContext = Procedure.ContextType == ContextType.Server;
			WindowUIDArgument = new ArgumentViewModel(ShowDialogArguments.WindowIDArgument, stepViewModel.Update, UpdateContent);
			WindowUIDArgument.Update(Procedure, ExplicitType.String);
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

		public bool CustomPosition
		{
			get { return ShowDialogArguments.CustomPosition; }
			set
			{
				ShowDialogArguments.CustomPosition = value;
				OnPropertyChanged(() => CustomPosition);
			}
		}
		public double Left
		{
			get { return ShowDialogArguments.Left; }
			set
			{
				ShowDialogArguments.Left = value;
				OnPropertyChanged(() => Left);
			}
		}
		public double Top
		{
			get { return ShowDialogArguments.Top; }
			set
			{
				ShowDialogArguments.Top = value;
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
				ShowDialogArguments.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override string Description
		{
			get
			{
				return string.Format("Открыть диалог: {0} {1}", SelectedLayout == null ? ArgumentViewModel.EmptyText : SelectedLayout.Name, IsModalWindow ? "(модальный)" : "(не модальный)");
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
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ShowDialogArguments.Layout);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowDialogArguments.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}
	}
}
