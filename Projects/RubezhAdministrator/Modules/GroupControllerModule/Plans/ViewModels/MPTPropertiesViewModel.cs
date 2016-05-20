using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class MPTPropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementMPT IElementMPT;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public MPTPropertiesViewModel(IElementMPT element, CommonDesignerCanvas designerCanvas)
		{
			IElementMPT = element;
			PositionSettingsViewModel = new PositionSettingsViewModel(element as ElementBase, designerCanvas);
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: МПТ";
			MPTs = new ObservableCollection<GKMPT>(GKManager.MPTs);
			if (IElementMPT.MPTUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.UID == IElementMPT.MPTUID);
		}
		public ObservableCollection<GKMPT> MPTs { get; private set; }

		private GKMPT _selectedMPT;
		public GKMPT SelectedMPT
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
			ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Publish(SelectedMPT.UID);
			MPTs = new ObservableCollection<GKMPT>(GKManager.MPTs);
			OnPropertyChanged(() => MPTs);
		}
		private bool CanEdit()
		{
			return SelectedMPT != null;
		}
		protected override bool Save()
		{
			PositionSettingsViewModel.SavePosition();
			GKPlanExtension.Instance.RewriteItem(IElementMPT, SelectedMPT);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedMPT != null;
		}
	}
}