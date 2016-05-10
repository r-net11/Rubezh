using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
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
		const int _sensivityFactor = 100;
		private IElementMPT IElementMPT;
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		public bool CanEditPosition { get; private set; }
		public MPTPropertiesViewModel(IElementMPT element)
		{
			IElementMPT = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			CanEditPosition = ElementBaseRectangle != null;
			if (CanEditPosition)
			{
				Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
				Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
			}
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: МПТ";
			MPTs = new ObservableCollection<GKMPT>(GKManager.MPTs);
			if (IElementMPT.MPTUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.UID == IElementMPT.MPTUID);
		}
		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
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
			ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Publish(SelectedMPT.UID);
		}
		private bool CanEdit()
		{
			return SelectedMPT != null;
		}
		protected override bool Save()
		{
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(IElementMPT, SelectedMPT);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedMPT != null;
		}
	}
}