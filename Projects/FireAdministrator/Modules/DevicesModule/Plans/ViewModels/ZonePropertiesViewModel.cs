using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using DevicesModule.Plans.Designer;

namespace DevicesModule.Plans.ViewModels
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
			Zones = new ObservableCollection<Zone>(FiresecManager.DeviceConfiguration.Zones);
			if (iElementZone.ZoneNo.HasValue)
				SelectedZone = Zones.FirstOrDefault(x => x.No == iElementZone.ZoneNo.Value);
		}

		public ObservableCollection<Zone> Zones { get; private set; }

		private Zone _selectedZone;
		public Zone SelectedZone
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
			var createZoneEventArg = new CreateZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Publish(createZoneEventArg);
			IElementZone.ZoneNo = createZoneEventArg.ZoneNo;
			Helper.SetZone(IElementZone);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Publish(SelectedZone.No);
			OnPropertyChanged("Zones");
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Helper.SetZone(IElementZone, SelectedZone);
			return base.Save();
		}
	}
}
