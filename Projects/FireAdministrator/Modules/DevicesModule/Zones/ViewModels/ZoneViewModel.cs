using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using System.Diagnostics;

namespace DevicesModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public Zone Zone { get; private set; }

		public ZoneViewModel(Zone zone)
		{
			Zone = zone;
		}

		public string Name
		{
			get { return Zone.Name; }
			set
			{
				Zone.Name = value;
				Zone.OnChanged();
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public string Description
		{
			get { return Zone.Description; }
			set
			{
				Zone.Description = value;
				Zone.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public string DetectorCount
		{
			get
			{
				if (Zone.ZoneType == ZoneType.Fire)
					return Zone.DetectorCount.ToString();
				return null;
			}
		}

		public void Update(Zone zone)
		{
			Zone = zone;
			OnPropertyChanged("Zone");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			OnPropertyChanged("DetectorCount");
		}
	}
}