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
			return logic;
		}
	}
}