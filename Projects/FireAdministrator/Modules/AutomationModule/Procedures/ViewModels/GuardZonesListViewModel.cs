using Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.Errors;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationModule.ViewModels
{
	public class GuardZonesListViewModel : SaveCancelDialogViewModel
	{
		private bool _isBisy;
		private List<OPCZone> _zones;
		private OPCZone _selectedZone;

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

		public OPCZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				if (_selectedZone == value) return;
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
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

		public GuardZonesListViewModel()
		{
			LoadZones();
		}

		public void LoadZones()
		{
			IsBisy = true;

			Task.Factory.StartNew(() => FiresecManager.FiresecService.GetGuardZones())
			.ContinueWith(t =>
			{
				IsBisy = false;
				if (t.IsFaulted || t.Result.HasError)
				{
					var ex = t.Exception;
					while (ex != null && ex.InnerException != null)
					{
						ex = (AggregateException) ex.InnerException;
						Logger.Error(ex);
					}
                    MessageBoxService.ShowError(CommonErrors.OPCScriptConnectionContent_Error);
				}
				else
				{
					Zones = t.Result.Result.ToList();
					SelectedZone = Zones.FirstOrDefault();
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override bool CanSave()
		{
			return !IsBisy;
		}
	}
}
