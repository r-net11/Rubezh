using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.ViewModels;
using FiresecAPI;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public ZoneDetailsViewModel(ZoneViewModel zoneViewModel)
		{
			Title = "Новая зона";
			ParentZoneViewModel = zoneViewModel;
			ParentZone = ParentZoneViewModel.Zone;
		}

		protected ZoneViewModel ParentZoneViewModel;
		protected SKDZone ParentZone;
		public ZoneViewModel AddedZone { get; protected set; }

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		protected override bool Save()
		{
			var zone = new SKDZone()
			{
				Name = Name,
				Description = Description
			};
			SKDManager.Zones.Add(zone);
			AddedZone = new ZoneViewModel(zone);
			ParentZoneViewModel.Zone.Children.Add(zone);
			ParentZoneViewModel.AddChild(AddedZone);
			ParentZoneViewModel.Update();
			SKDManager.SKDConfiguration.Update();
			return true;
		}
	}
}