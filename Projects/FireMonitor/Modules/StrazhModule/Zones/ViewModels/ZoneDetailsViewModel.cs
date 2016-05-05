using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using StrazhModule.Zones;

namespace StrazhModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State
		{
			get { return Zone.State; }
		}

		public ZoneDetailsViewModel(SKDZone zone)
		{
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			ZoneAccessStateNormalCommand = new RelayCommand(OnZoneAccessStateNormal, CanZoneAccessStateNormal);
			ZoneAccessStateCloseAlwaysCommand = new RelayCommand(OnZoneAccessStateCloseAlways, CanZoneAccessStateCloseAlways);
			ZoneAccessStateOpenAlwaysCommand = new RelayCommand(OnZoneAccessStateOpenAlways, CanZoneAccessStateOpenAlways);

			Zone = zone;
			Title = Zone.Name;
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsPromptWarning);
			CommandManager.InvalidateRequerySuggested();
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				elementBase = plan.ElementRectangleSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public bool IsPromptWarning
		{
			get
			{
				//return State.StateClass == XStateClass.Attention;
				return true;
			}
		}

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			ZoneCommander.ClearPromptWarning(Zone);
		}
		private bool CanClearPromptWarning()
		{
			return ZoneCommander.CanClearPromptWarning(Zone);
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			ZoneCommander.Open(Zone);
		}
		bool CanOpen()
		{
			return ZoneCommander.CanOpen(Zone);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			ZoneCommander.Close(Zone);
		}
		bool CanClose()
		{
			return ZoneCommander.CanClose(Zone);
		}

		public RelayCommand ZoneAccessStateNormalCommand { get; private set; }
		void OnZoneAccessStateNormal()
		{
			ZoneCommander.SetAccessStateToNormal(Zone);
		}
		bool CanZoneAccessStateNormal()
		{
			return ZoneCommander.CanSetAccessStateToNormal(Zone);
		}

		public RelayCommand ZoneAccessStateCloseAlwaysCommand { get; private set; }
		void OnZoneAccessStateCloseAlways()
		{
			ZoneCommander.SetAccessStateToCloseAlways(Zone);
		}
		bool CanZoneAccessStateCloseAlways()
		{
			return ZoneCommander.CanSetAccessStateToCloseAlways(Zone);
		}

		public RelayCommand ZoneAccessStateOpenAlwaysCommand { get; private set; }
		void OnZoneAccessStateOpenAlways()
		{
			ZoneCommander.SetAccessStateToOpenAlways(Zone);
		}
		bool CanZoneAccessStateOpenAlways()
		{
			return ZoneCommander.CanSetAccessStateToOpenAlways(Zone);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDZone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Zone.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}