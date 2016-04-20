using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class CodesSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKCode> Codes { get; private set; }
		public bool CanCreateNew { get; private set; }

		public CodesSelectationViewModel(List<GKCode> codes, bool canCreateNew = true)
		{
			Title = "Выбор кодов";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
			CreateNewCommand = new RelayCommand(OnCreateNew);

			Codes = codes;
			CanCreateNew = canCreateNew;
			TargetCodes = new ObservableCollection<GKCode>();
			SourceCodes = new ObservableCollection<GKCode>();

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (Codes.Contains(code))
					TargetCodes.Add(code);
				else
					SourceCodes.Add(code);
			}

			SelectedTargetCode = TargetCodes.FirstOrDefault();
			SelectedSourceCode = SourceCodes.FirstOrDefault();
		}

		public ObservableCollection<GKCode> SourceCodes { get; private set; }

		GKCode _selectedSourceCode;
		public GKCode SelectedSourceCode
		{
			get { return _selectedSourceCode; }
			set
			{
				_selectedSourceCode = value;
				OnPropertyChanged(() => SelectedSourceCode);
			}
		}

		public ObservableCollection<GKCode> TargetCodes { get; private set; }

		GKCode _selectedTargetCode;
		public GKCode SelectedTargetCode
		{
			get { return _selectedTargetCode; }
			set
			{
				_selectedTargetCode = value;
				OnPropertyChanged(() => SelectedTargetCode);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceCodes;
		void OnAdd(object parameter)
		{
			var index = SourceCodes.IndexOf(SelectedSourceCode);

			SelectedSourceCodes = (IList)parameter;
			var codeViewModels = new List<GKCode>();
			foreach (var selectedCode in SelectedSourceCodes)
			{
				var codeViewModel = selectedCode as GKCode;
				if (codeViewModel != null)
					codeViewModels.Add(codeViewModel);
			}
			foreach (var codeViewModel in codeViewModels)
			{
				TargetCodes.Add(codeViewModel);
				SourceCodes.Remove(codeViewModel);
			}
			SelectedTargetCode = TargetCodes.LastOrDefault();
			OnPropertyChanged(() => SourceCodes);

			index = Math.Min(index, SourceCodes.Count - 1);
			if (index > -1)
				SelectedSourceCode = SourceCodes[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetCodes;
		void OnRemove(object parameter)
		{
			var index = TargetCodes.IndexOf(SelectedTargetCode);

			SelectedTargetCodes = (IList)parameter;
			var codeViewModels = new List<GKCode>();
			foreach (var selectedCode in SelectedTargetCodes)
			{
				var codeViewModel = selectedCode as GKCode;
				if (codeViewModel != null)
					codeViewModels.Add(codeViewModel);
			}
			foreach (var codeViewModel in codeViewModels)
			{
				SourceCodes.Add(codeViewModel);
				TargetCodes.Remove(codeViewModel);
			}
			SelectedSourceCode = SourceCodes.LastOrDefault();
			OnPropertyChanged(() => TargetCodes);

			index = Math.Min(index, TargetCodes.Count - 1);
			if (index > -1)
				SelectedTargetCode = TargetCodes[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var codeViewModel in SourceCodes)
			{
				TargetCodes.Add(codeViewModel);
			}
			SourceCodes.Clear();
			SelectedTargetCode = TargetCodes.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var codeViewModel in TargetCodes)
			{
				SourceCodes.Add(codeViewModel);
			}
			TargetCodes.Clear();
			SelectedSourceCode = SourceCodes.FirstOrDefault();
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			var createCodeEventArg = new CreateGKCodeEventArg();
			ServiceFactory.Events.GetEvent<CreateGKCodeEvent>().Publish(createCodeEventArg);
			if (createCodeEventArg.Code != null)
			{
				TargetCodes.Add(createCodeEventArg.Code);
				OnPropertyChanged(() => TargetCodes);
			}
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceCode != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetCode != null;
		}

		public bool CanAddAll()
		{
			return (SourceCodes.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetCodes.Count > 0);
		}

		protected override bool Save()
		{
			Codes = new List<GKCode>(TargetCodes);
			return base.Save();
		}
	}
}