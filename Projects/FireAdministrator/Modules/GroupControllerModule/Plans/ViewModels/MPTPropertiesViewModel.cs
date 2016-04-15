using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using RubezhAPI;

namespace GKModule.Plans.ViewModels
{
	public class MPTPropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementMPT _element;
		private MPTsViewModel _mptsViewModel;

		public MPTPropertiesViewModel(IElementMPT element, MPTsViewModel mptsViewModel)
		{
			_mptsViewModel = mptsViewModel;
			_element = element;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: МПТ";
			MPTs = new ObservableCollection<MPTViewModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				var mptViewModel = new MPTViewModel(mpt);
				MPTs.Add(mptViewModel);
			}
			if (_element.MPTUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == _element.MPTUID);
		}

		public ObservableCollection<MPTViewModel> MPTs { get; private set; }

		private MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				OnPropertyChanged(() => SelectedMPT);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid mptUID = _element.MPTUID;
			var createMPTEventArg = new CreateGKMPTEventArg();
			ServiceFactory.Events.GetEvent<CreateGKMPTEvent>().Publish(createMPTEventArg);
			if (createMPTEventArg.MPT != null)
				_element.MPTUID = createMPTEventArg.MPT.UID;
			GKPlanExtension.Instance.Cache.BuildSafe<GKMPT>();
			GKPlanExtension.Instance.SetItem<GKMPT>(_element);
			UpdateMPTs(mptUID);
			if (!createMPTEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Publish(SelectedMPT.MPT.UID);
			SelectedMPT.Update();
		}
		private bool CanEdit()
		{
			return SelectedMPT != null;
		}

		protected override bool Save()
		{
			Guid mptUID = _element.MPTUID;
			GKPlanExtension.Instance.SetItem<GKMPT>(_element, SelectedMPT == null ? null : SelectedMPT.MPT);
			UpdateMPTs(mptUID);
			return base.Save();
		}
		private void UpdateMPTs(Guid mptUID)
		{
			if (_mptsViewModel != null)
			{
				if (mptUID != _element.MPTUID)
					Update(mptUID);
				Update(_element.MPTUID);
				_mptsViewModel.LockedSelect(_element.MPTUID);
			}
		}
		private void Update(Guid mptUID)
		{
			var mpt = _mptsViewModel.MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
			if (mpt != null)
				mpt.Update();
		}
	}
}