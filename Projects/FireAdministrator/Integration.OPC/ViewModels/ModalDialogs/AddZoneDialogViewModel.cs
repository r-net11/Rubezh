using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization.IntegrationOPC.Errors;
using Localization.IntegrationOPC.ViewModels;

namespace Integration.OPC.ViewModels
{
	public class AddZoneDialogViewModel : SaveCancelDialogViewModel
	{
		private bool _isBisy;
		private readonly IEnumerable<OPCZone> _existingZones;
		private List<OPCZone> _zones;

		public bool IsBisy
		{
			get { return _isBisy; }
			set
			{
				if (_isBisy == value) return;
				_isBisy = value;
				OnPropertyChanged(() => IsBisy);
			}
		}

		public List<OPCZone> Zones
		{
			get { return _zones; }
			set
			{
				if (_zones == value) return;
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		public AddZoneDialogViewModel(IEnumerable<OPCZone> existingZones)
		{
			Title = CommonViewModels.ZonesOPC;
			_existingZones = existingZones;
			LoadingContent();
		}

		private void LoadingContent()
		{
			IsBisy = true;
			Task.Factory
				.StartNew(() => FiresecManager.FiresecService.GetOPCZones())
				.ContinueWith(t =>
				{
					IsBisy = false;

					if (t.IsFaulted || t.Result == null || t.Result.HasError)
						MessageBoxService.ShowError(CommonErrors.GetOPCZonesContent_Error);
					else
					{
						Zones = t.Result.Result.Select(newZone => new OPCZone(newZone, _existingZones.Any(zone => zone.No == newZone.No))).ToList();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
