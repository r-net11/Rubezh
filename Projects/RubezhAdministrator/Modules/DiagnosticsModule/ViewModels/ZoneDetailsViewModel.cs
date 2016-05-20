using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagnosticsModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKZone Zone { get; private set; }
		public ZoneDetailsViewModel(GKZone zone)
		{
			Title = "Добавление зоны";
			if (zone == null)
				zone = new GKZone();
			Zone = zone;
			Name = Zone.Name;
			Description = Zone.Description;
			Fire1Count = Zone.Fire1Count;
			Fire2Count = Zone.Fire2Count;
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public int Fire1Count { get; set; }
		public int Fire2Count { get; set; }

		protected override bool Save()
		{
			Zone.Name = Name;
			Zone.Description = Description;
			Zone.Fire1Count = Fire1Count;
			Zone.Fire2Count = Fire2Count;
			return base.Save();
		}
	}
}