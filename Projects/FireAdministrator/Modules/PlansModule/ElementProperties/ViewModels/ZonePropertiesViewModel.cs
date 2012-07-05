using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using PlansModule.Designer.Designer;

namespace PlansModule.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;

		public ZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			Title = "Свойства фигуры: Зона";
			Zones = new List<Zone>(FiresecManager.DeviceConfiguration.Zones);
			if (iElementZone.ZoneNo.HasValue)
				SelectedZone = Zones.FirstOrDefault(x => x.No == iElementZone.ZoneNo.Value);
		}

		public List<Zone> Zones { get; private set; }

		Zone _selectedZone;
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
		void OnCreate()
		{
			var createZoneEventArg = new CreateZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Publish(createZoneEventArg);
			IElementZone.ZoneNo = createZoneEventArg.ZoneNo;
			Helper.SetZone(IElementZone);
			if (createZoneEventArg.Cancel == false)
				Close(true);
		}

		protected override bool Save()
		{
			Helper.SetZone(IElementZone, SelectedZone);
			return base.Save();
		}
	}
}
