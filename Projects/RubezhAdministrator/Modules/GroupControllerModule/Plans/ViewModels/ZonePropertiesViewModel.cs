using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		IElementZone IElementZone;
		ElementBase ElementBase { get; set; }
		public ZonePropertiesViewModel(IElementZone element)
		{
			IElementZone = element;
			ElementBase = element as ElementBase;
			var position = ElementBase.GetPosition();
			Left = (int)(position.X * _sensivityFactor);
			Top = (int)(position.Y * _sensivityFactor);
			Title = "Свойства фигуры: Пожарная зона";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Zones = new ObservableCollection<GKZone>(GKManager.Zones);
			if (element.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == element.ZoneUID);

			ShowState = IElementZone.ShowState;
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
		public ObservableCollection<GKZone> Zones { get; private set; }

		GKZone _selectedZone;
		public GKZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		bool _showState;
		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		void OnCreate()
		{
			var createZoneEventArg = new CreateGKZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createZoneEventArg.Zone);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Publish(SelectedZone.UID);
			Zones = new ObservableCollection<GKZone>(GKManager.Zones);
			OnPropertyChanged(() => Zones);
		}
		bool CanEdit()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			IElementZone.ShowState = ShowState;
			ElementBase.SetPosition(new System.Windows.Point((double)Left / _sensivityFactor, (double)Top / _sensivityFactor));
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}