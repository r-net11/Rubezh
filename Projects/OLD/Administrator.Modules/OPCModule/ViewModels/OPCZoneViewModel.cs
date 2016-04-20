using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;

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
				OnPropertyChanged(() => IsOPCUsed);
				ServiceFactory.SaveService.OPCChanged = true;
				ServiceFactory.SaveService.FSChanged = true;
			}
		}
	}
}