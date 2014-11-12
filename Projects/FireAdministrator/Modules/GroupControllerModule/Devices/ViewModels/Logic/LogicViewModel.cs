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
		public GKDevice Device { get; private set; }

		public LogicViewModel(GKDevice device, GKLogic logic, bool hasOffClause, bool hasStopClause)
		{
			if (device.DriverType == GKDriverType.System)
				Title = "Настройка логики";
			else
				Title = "Настройка логики устройства " + device.PresentationName;
			Device = device;

			if (logic.OnClausesGroup.Clauses.Count == 0)
			{
				logic.OnClausesGroup.Clauses.Add(new GKClause());
			}

			OnClausesGroup = new ClauseGroupViewModel(device, logic.OnClausesGroup);
			OffClausesGroup = new ClauseGroupViewModel(device, logic.OffClausesGroup);
			StopClausesGroup = new ClauseGroupViewModel(device, logic.StopClausesGroup);

			SelectedMROMessageNo = logic.ZoneLogicMROMessageNo;
			SelectedMROMessageType = logic.ZoneLogicMROMessageType;

			HasOffClause = hasOffClause;
			HasStopClause = hasStopClause;
		}

		public LogicViewModel _deviceDetailsViewModel { get; private set; }

		public ClauseGroupViewModel OnClausesGroup { get; private set; }
		public ClauseGroupViewModel OffClausesGroup { get; private set; }
		public ClauseGroupViewModel StopClausesGroup { get; private set; }

		#region IsMRO_2M
		public bool IsMRO_2M
		{
			get { return Device.DriverType == GKDriverType.MRO_2; }
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

		public bool HasOffClause { get; private set; }
		public bool HasStopClause { get; private set; }

		public string OffClauseName
		{
			get
			{
				if ((Device.Logic.OffClausesGroup.Clauses == null || Device.Logic.OffClausesGroup.Clauses.Count == 0) && HasStopClause == false)
					return "Условие выключения противоположно условию включения";
				else
					return "Условие выключения";
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
			logic.StopClausesGroup = StopClausesGroup.GetClauseGroup();
			logic.ZoneLogicMROMessageNo = SelectedMROMessageNo;
			logic.ZoneLogicMROMessageType = SelectedMROMessageType;
			return logic;
		}
	}
}