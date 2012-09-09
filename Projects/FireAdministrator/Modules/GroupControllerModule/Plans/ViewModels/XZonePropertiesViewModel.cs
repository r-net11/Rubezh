using System.Collections.ObjectModel;
using System.Linq;
using GKModule.Plans.Designer;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class XZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;

		public XZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Зона";
			Zones = new ObservableCollection<XZone>(XManager.DeviceConfiguration.Zones);
			if (iElementZone.ZoneNo.HasValue)
				SelectedZone = Zones.FirstOrDefault(x => x.No == iElementZone.ZoneNo.Value);
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
			IElementZone.ZoneNo = createZoneEventArg.ZoneNo;
			Helper.SetXZone(IElementZone);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditXZoneEvent>().Publish(SelectedZone.No);
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
