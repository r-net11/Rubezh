using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class LogicViewModel : SaveCancelDialogViewModel
	{
		public ClauseGroupViewModel OnClausesGroup { get; private set; }
		public ClauseGroupViewModel OffClausesGroup { get; private set; }
		public ClauseGroupViewModel OnNowClausesGroup { get; private set; }
		public ClauseGroupViewModel OffNowClausesGroup { get; private set; }
		public ClauseGroupViewModel StopClausesGroup { get; private set; }
		public bool HasOffClause { get; private set; }
		public bool HasStopClause { get; private set; }
		public bool HasOnNowClause { get; private set; }
		public bool HasOffNowClause { get; private set; }

		public LogicViewModel(GKDevice device, GKLogic logic, bool hasOffClause = false, bool hasOnNowClause = false, bool hasOffNowClause = false, bool hasStopClause = false)
		{
			if (device != null)
				Title = "Настройка логики устройства " + device.PresentationName;
			else
				Title = "Настройка логики";

			if (logic.OnClausesGroup.Clauses.Count == 0 && logic.OnClausesGroup.ClauseGroups.Count == 0)
			{
				logic.OnClausesGroup.Clauses.Add(new GKClause());
			}

			OnClausesGroup = new ClauseGroupViewModel(device, logic.OnClausesGroup);
			OffClausesGroup = new ClauseGroupViewModel(device, logic.OffClausesGroup);
			OnNowClausesGroup = new ClauseGroupViewModel(device, logic.OnNowClausesGroup);
			OffNowClausesGroup = new ClauseGroupViewModel(device, logic.OffNowClausesGroup);
			StopClausesGroup = new ClauseGroupViewModel(device, logic.StopClausesGroup);
			UseOffCounterLogic = logic.UseOffCounterLogic;

			HasOffClause = hasOffClause;
			HasOnNowClause = hasOnNowClause;
			HasOffNowClause = hasOffNowClause;
			HasStopClause = hasStopClause;

			SelectedMROMessageNo = logic.ZoneLogicMROMessageNo;
			SelectedMROMessageType = logic.ZoneLogicMROMessageType;
		}

		bool _useOffCounterLogic;
		public bool UseOffCounterLogic
		{
			get { return _useOffCounterLogic; }
			set
			{
				_useOffCounterLogic = value;
				OnPropertyChanged(() => UseOffCounterLogic);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}

		public GKLogic GetModel()
		{
			var logic = new GKLogic();
			logic.OnClausesGroup = OnClausesGroup.GetClauseGroup();
			logic.OffClausesGroup = OffClausesGroup.GetClauseGroup();
			logic.OnNowClausesGroup = OnNowClausesGroup.GetClauseGroup();
			logic.OffNowClausesGroup = OffNowClausesGroup.GetClauseGroup();
			logic.StopClausesGroup = StopClausesGroup.GetClauseGroup();
			logic.UseOffCounterLogic = UseOffCounterLogic;
			logic.ZoneLogicMROMessageNo = SelectedMROMessageNo;
			logic.ZoneLogicMROMessageType = SelectedMROMessageType;
			return logic;
		}

		#region IsMRO_2M
		public bool IsMRO_2M
		{
			//get { return Device.DriverType == GKDriverType.MRO_2; }
			get { return false; }
		}

		public List<ZoneLogicMROMessageNo> AvailableMROMessageNos
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageNo)).Cast<ZoneLogicMROMessageNo>().ToList(); }
		}

		ZoneLogicMROMessageNo _selectedMROMessageNo;
		public ZoneLogicMROMessageNo SelectedMROMessageNo
		{
			get { return _selectedMROMessageNo; }
			set
			{
				_selectedMROMessageNo = value;
				OnPropertyChanged(() => SelectedMROMessageNo);
			}
		}

		public List<ZoneLogicMROMessageType> AvailableMROMessageTypes
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageType)).Cast<ZoneLogicMROMessageType>().ToList(); }
		}

		ZoneLogicMROMessageType _selectedMROMessageType;
		public ZoneLogicMROMessageType SelectedMROMessageType
		{
			get { return _selectedMROMessageType; }
			set
			{
				_selectedMROMessageType = value;
				OnPropertyChanged(() => SelectedMROMessageType);
			}
		}
		#endregion
	}
}