using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class MPTPropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementMPT IElementMPT;

		public MPTPropertiesViewModel(IElementMPT element)
		{
			IElementMPT = element;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: МПТ";
			MPTs = new ObservableCollection<MPTViewModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				var mptViewModel = new MPTViewModel(mpt);
				MPTs.Add(mptViewModel);
			}
			if (IElementMPT.MPTUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == IElementMPT.MPTUID);
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
			Guid mptUID = IElementMPT.MPTUID;
			var createMPTEventArg = new CreateGKMPTEventArg();
			ServiceFactory.Events.GetEvent<CreateGKMPTEvent>().Publish(createMPTEventArg);
			if (createMPTEventArg.MPT != null)
				GKPlanExtension.Instance.RewriteItem(IElementMPT, createMPTEventArg.MPT);
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
			GKPlanExtension.Instance.RewriteItem(IElementMPT, SelectedMPT.MPT);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedMPT != null;
		}
	}
}