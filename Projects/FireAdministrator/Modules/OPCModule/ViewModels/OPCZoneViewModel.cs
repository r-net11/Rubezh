using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;

namespace OPCModule.ViewModels
{
	public class OPCZoneViewModel : BaseViewModel
	{
		public Zone Zone { get; private set; }

		public OPCZoneViewModel(Zone zone)
		{
			Zone = zone;
		}

		public bool IsOPCUsed
		{
			get { return Zone.IsOPCUsed; }
			set
			{
				Zone.IsOPCUsed = value;
				OnPropertyChanged("IsOPCUsed");
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}
	}
}