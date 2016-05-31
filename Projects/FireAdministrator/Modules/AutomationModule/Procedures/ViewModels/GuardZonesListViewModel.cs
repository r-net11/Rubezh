using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomationModule.Properties;
using Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
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
			Task.Factory.StartNew(() => FiresecManager.FiresecService.GetOPCZones())
			.ContinueWith(t =>
			{
				if (t.IsFaulted || t.Result.HasError)
				{
					IsBisy = false;
					var ex = t.Exception;
					while (ex is AggregateException && ex.InnerException != null)
					{
						ex = (AggregateException) ex.InnerException;
						Logger.Error(ex);
					}
					MessageBoxService.ShowError(Resources.ErrorOPCScriptConnectionContent);
				}
				else
				{
					IsBisy = false;
					Zones = t.Result.Result.Where(x => x.Type == OPCZoneType.Guard).ToList();
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
