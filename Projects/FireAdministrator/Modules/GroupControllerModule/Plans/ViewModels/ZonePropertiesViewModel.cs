using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;

		public ZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Зона";
			Zones = new ObservableCollection<XZone>(XManager.DeviceConfiguration.SortedZones);
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<XZone> Zones { get; private set; }

		private XZone _selectedZone;
		public XZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createZoneEventArg = new CreateXZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateXZoneEvent>().Publish(createZoneEventArg);
			IElementZone.ZoneUID = createZoneEventArg.ZoneUID;
			Helper.SetXZone(IElementZone);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Publish(SelectedZone.UID);
			OnPropertyChanged("Zones");
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Helper.SetXZone(IElementZone, SelectedZone);
			return base.Save();
		}
	}
}