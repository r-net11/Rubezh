using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class MPTsSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<XMPT> MPTs { get; private set; }
		public bool CanCreateNew { get; private set; }

		public MPTsSelectationViewModel(List<XMPT> mpts, bool canCreateNew = false)
		{
			Title = "Выбор МПТ";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			MPTs = mpts;
			CanCreateNew = canCreateNew;
			TargetMPTs = new ObservableCollection<XMPT>();
			SourceMPTs = new ObservableCollection<XMPT>();

			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (MPTs.Contains(mpt))
					TargetMPTs.Add(mpt);
				else
					SourceMPTs.Add(mpt);
			}

			SelectedTargetMPT = TargetMPTs.FirstOrDefault();
			SelectedSourceMPT = SourceMPTs.FirstOrDefault();
		}

		public ObservableCollection<XMPT> SourceMPTs { get; private set; }

		XMPT _selectedSourceMPT;
		public XMPT SelectedSourceMPT
		{
			get { return _selectedSourceMPT; }
			set
			{
				_selectedSourceMPT = value;
				OnPropertyChanged("SelectedSourceMPT");
			}
		}

		public ObservableCollection<XMPT> TargetMPTs { get; private set; }

		XMPT _selectedTargetMPT;
		public XMPT SelectedTargetMPT
		{
			get { return _selectedTargetMPT; }
			set
			{
				_selectedTargetMPT = value;
				OnPropertyChanged("SelectedTargetMPT");
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceMPTs;
		void OnAdd(object parameter)
		{
			var index = SourceMPTs.IndexOf(SelectedSourceMPT);

			SelectedSourceMPTs = (IList)parameter;
			var mptViewModels = new List<XMPT>();
			foreach (var selectedMPT in SelectedSourceMPTs)
			{
				var mptViewModel = selectedMPT as XMPT;
				if (mptViewModel != null)
					mptViewModels.Add(mptViewModel);
			}
			foreach (var mptViewModel in mptViewModels)
			{
				TargetMPTs.Add(mptViewModel);
				SourceMPTs.Remove(mptViewModel);
			}
			SelectedTargetMPT = TargetMPTs.LastOrDefault();
			OnPropertyChanged("SourceMPTs");

			index = Math.Min(index, SourceMPTs.Count - 1);
			if (index > -1)
				SelectedSourceMPT = SourceMPTs[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetMPTs;
		void OnRemove(object parameter)
		{
			var index = TargetMPTs.IndexOf(SelectedTargetMPT);

			SelectedTargetMPTs = (IList)parameter;
			var mptViewModels = new List<XMPT>();
			foreach (var selectedMPT in SelectedTargetMPTs)
			{
				var mptViewModel = selectedMPT as XMPT;
				if (mptViewModel != null)
					mptViewModels.Add(mptViewModel);
			}
			foreach (var mptViewModel in mptViewModels)
			{
				SourceMPTs.Add(mptViewModel);
				TargetMPTs.Remove(mptViewModel);
			}
			SelectedSourceMPT = SourceMPTs.LastOrDefault();
			OnPropertyChanged("TargetMPTs");

			index = Math.Min(index, TargetMPTs.Count - 1);
			if (index > -1)
				SelectedTargetMPT = TargetMPTs[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var mptViewModel in SourceMPTs)
			{
				TargetMPTs.Add(mptViewModel);
			}
			SourceMPTs.Clear();
			SelectedTargetMPT = TargetMPTs.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var mptViewModel in TargetMPTs)
			{
				SourceMPTs.Add(mptViewModel);
			}
			TargetMPTs.Clear();
			SelectedSourceMPT = SourceMPTs.FirstOrDefault();
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceMPT != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetMPT != null;
		}

		public bool CanAddAll()
		{
			return (SourceMPTs.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetMPTs.Count > 0);
		}

		protected override bool Save()
		{
			MPTs = new List<XMPT>(TargetMPTs);
			return base.Save();
		}
	}
}