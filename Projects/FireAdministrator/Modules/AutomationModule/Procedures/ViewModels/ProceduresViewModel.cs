using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutomationModule.Plans;
using Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;

namespace AutomationModule.ViewModels
{
	public class ProceduresViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		private SortableObservableCollection<ProcedureViewModel> _procedures;
		private ProcedureViewModel _selectedProcedure;
		private Procedure _procedureToCopy;
		private bool _lockSelection;

		public ProceduresViewModel()
		{
			Current = this;
			Menu = new ProceduresMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedProcedure != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedProcedure != null);
			CopyCommand = new RelayCommand(OnCopy, () => SelectedProcedure != null);
			PasteCommand = new RelayCommand(OnPaste, () => _procedureToCopy != null);
			CutCommand = new RelayCommand(OnCut, () => SelectedProcedure != null);
			RegisterShortcuts();
			SetRibbonItems();
			SubscribeEvents();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = false;
		}

		public static ProceduresViewModel Current { get; private set; }

		public SortableObservableCollection<ProcedureViewModel> Procedures
		{
			get { return _procedures; }
			set
			{
				_procedures = value;
				OnPropertyChanged(() => Procedures);
			}
		}

		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
				GenerateFindEvent();

				if (value == null) return;

				value.StepsViewModel.SelectedStep = value.StepsViewModel.AllSteps.FirstOrDefault();
				value.StepsViewModel.UpdateContent();
			}
		}

		public void Initialize()
		{
			Procedures = new SortableObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();

			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}

			Procedures.Sort(x => x.Name);
			SelectedProcedure = Procedures.FirstOrDefault();
		}

		private void OnCopy()
		{
			_procedureToCopy = Utils.Clone(SelectedProcedure.Procedure);
		}

		private void OnCut()
		{
			OnCopy();
			OnDelete();
		}

		private void OnPaste()
		{
			_procedureToCopy.Uid = Guid.NewGuid();
			var procedureViewModel = new ProcedureViewModel(Utils.Clone(_procedureToCopy));
			FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureViewModel.Procedure);
			Procedures.Add(procedureViewModel);
			SelectedProcedure = procedureViewModel;
			ServiceFactory.SaveService.AutomationChanged = true;
			AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
		}

		private void OnAdd()
		{
			var procedureDetailsViewModel = new ProcedureDetailsViewModel();
			if (DialogService.ShowModalWindow(procedureDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureDetailsViewModel.Procedure);
				var procedureViewModel = new ProcedureViewModel(procedureDetailsViewModel.Procedure);
				Procedures.Add(procedureViewModel);
				SelectedProcedure = procedureViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
				AutomationPlanExtension.Instance.Cache.BuildSafe<Procedure>();
			}
		}

		private void OnEdit()
		{
			var procedureDetailsViewModel = new ProcedureDetailsViewModel(SelectedProcedure.Procedure);
			if (DialogService.ShowModalWindow(procedureDetailsViewModel))
			{
				SelectedProcedure.Update(procedureDetailsViewModel.Procedure);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		private void OnDelete()
		{
			var index = Procedures.IndexOf(SelectedProcedure);
			FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
			Procedures.Remove(SelectedProcedure);
			index = Math.Min(index, Procedures.Count - 1);
			if (index > -1)
				SelectedProcedure = Procedures[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public void Select(Guid inputUid)
		{
			if (inputUid == Guid.Empty)
				return;
			var variables = new List<Variable>();
			var arguments = new List<Variable>();
			var procedureSteps = new List<ProcedureStep>();

			foreach (var procedure in Procedures)
			{
				variables.AddRange(procedure.Procedure.Variables);
				arguments.AddRange(procedure.Procedure.Arguments);
				procedureSteps.AddRange(GetAllSteps(procedure.Procedure.Steps, new List<ProcedureStep>()));
			}

			if (Procedures.Any(item => item.Procedure.Uid == inputUid))
			{
				SelectedProcedure = Procedures.FirstOrDefault(item => item.Procedure.Uid == inputUid);
			}
			else if (variables.Any(item => item.Uid == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Variables.Any(z => z.Uid == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowVariablesCommand.Execute();
				}
				SelectedProcedure.VariablesViewModel.SelectedVariable = SelectedProcedure.VariablesViewModel.Variables.FirstOrDefault(item => item.Variable.Uid == inputUid);
			}
			else if (arguments.Any(item => item.Uid == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Arguments.Any(z => z.Uid == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowArgumentsCommand.Execute();
				}
				SelectedProcedure.ArgumentsViewModel.SelectedVariable = SelectedProcedure.ArgumentsViewModel.Variables.FirstOrDefault(item => item.Variable.Uid == inputUid);
			}

			else if (procedureSteps.Any(item => item.UID == inputUid))
			{
				var selectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Steps.Any(z => z.UID == inputUid));
				if (selectedProcedure != null)
				{
					SelectedProcedure = selectedProcedure;
					SelectedProcedure.ShowStepsCommand.Execute();
				}
				SelectedProcedure.StepsViewModel.SelectedStep = SelectedProcedure.StepsViewModel.AllSteps.FirstOrDefault(item => item.Step.UID == inputUid);
			}
		}

		private static List<ProcedureStep> GetAllSteps(IEnumerable<ProcedureStep> steps, List<ProcedureStep> resultSteps)
		{
			foreach (var step in steps)
			{
				resultSteps.Add(step);
				GetAllSteps(step.Children, resultSteps);
			}
			return resultSteps;
		}

		public override void OnShow()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;

			if (SelectedProcedure != null)
				SelectedProcedure.StepsViewModel.UpdateContent();

			ServiceFactory.SaveService.AutomationChanged = automationChanged;

			if (Procedures != null)
				Procedures.Sort(x => x.Name);

			base.OnShow();
			SelectedProcedure = Procedures != null ? Procedures.FirstOrDefault() : null;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.X, ModifierKeys.Control), CutCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.E, ModifierKeys.Control), EditCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>
			{
				new RibbonMenuItemViewModel(CommonViewModels.Procedures_Edition, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Add, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Copy, CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Cut, CutCommand, "BCut"),
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Paste, PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel(CommonViewModels.Procedures_Delete, DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
        }
		private void SubscribeEvents()
		{
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}

		private void OnProcedureChanged(Guid uid)
		{
			var procedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == uid);
			if (procedure == null) return;

			procedure.Update();

			if (!_lockSelection)
			{
				SelectedProcedure = procedure;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementProcedure = element as ElementProcedure;
				if (elementProcedure != null)
					OnProcedureChanged(elementProcedure.ProcedureUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementProcedure = element as ElementProcedure;
			if (elementProcedure == null) return;

			_lockSelection = true;
			Select(elementProcedure.ProcedureUID);
			_lockSelection = false;
		}

		/// <summary>
		/// Служит для уведомления о смене выбранной процедуры.
		/// Генерируемое событие принимает PlansViewModel для нахождения элемента на плане и осуществления принудительного выбора элемента на плане.
		/// </summary>
		private void GenerateFindEvent()
		{
			if (SelectedProcedure == null || _lockSelection || SelectedProcedure.Procedure == null) return;

			if (!SelectedProcedure.Procedure.PlanElementUIDs.Any()) return;

			ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(SelectedProcedure.Procedure.PlanElementUIDs);
		}

		public RelayCommand EditCommand { get; private set; }
		public RelayCommand DeleteCommand { get; private set; }
		public RelayCommand AddCommand { get; private set; }
		public RelayCommand CutCommand { get; private set; }
		public RelayCommand CopyCommand { get; private set; }
		public RelayCommand PasteCommand { get; private set; }
	}
}