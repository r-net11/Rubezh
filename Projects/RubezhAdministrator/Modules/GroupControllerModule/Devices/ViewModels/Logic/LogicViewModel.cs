using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class LogicViewModel : SaveCancelDialogViewModel
	{
		public ClauseGroupViewModel OnClausesGroup { get; private set; }
		public ClauseGroupViewModel On2ClausesGroup { get; private set; }
		public ClauseGroupViewModel OffClausesGroup { get; private set; }
		public ClauseGroupViewModel OnNowClausesGroup { get; private set; }
		public ClauseGroupViewModel OffNowClausesGroup { get; private set; }
		public ClauseGroupViewModel StopClausesGroup { get; private set; }
		public bool HasOnClause { get; private set; }
		public bool HasOn2Clause { get; private set; }
		public bool HasOffClause { get; private set; }
		public bool HasStopClause { get; private set; }
		public bool HasOnNowClause { get; private set; }
		public bool HasOffNowClause { get; private set; }

		public LogicViewModel(GKBase gkBase, GKLogic logic, bool hasOffClause = false, bool hasOnNowClause = false, bool hasOffNowClause = false, bool hasStopClause = false, bool hasOnClause = true, bool hasOn2Clause = false)
		{
			if (gkBase != null)
				Title = "Настройка логики " + gkBase.PresentationName;
			else
				Title = "Настройка логики";

			if (logic.OnClausesGroup.Clauses.Count == 0 && logic.OnClausesGroup.ClauseGroups.Count == 0 && hasOnClause)
			{
				logic.OnClausesGroup.Clauses.Add(new GKClause());
			}

			OnClausesGroup = new ClauseGroupViewModel(gkBase, logic.OnClausesGroup);
			On2ClausesGroup = new ClauseGroupViewModel(gkBase, logic.On2ClausesGroup);
			OffClausesGroup = new ClauseGroupViewModel(gkBase, logic.OffClausesGroup);
			OnNowClausesGroup = new ClauseGroupViewModel(gkBase, logic.OnNowClausesGroup);
			OffNowClausesGroup = new ClauseGroupViewModel(gkBase, logic.OffNowClausesGroup);
			StopClausesGroup = new ClauseGroupViewModel(gkBase, logic.StopClausesGroup);	
			UseOffCounterLogic = logic.UseOffCounterLogic;

			HasOnClause = hasOnClause;
			HasOffClause = hasOffClause;
			HasOnNowClause = hasOnNowClause;
			HasOffNowClause = hasOffNowClause;
			HasStopClause = hasStopClause;
			HasOn2Clause = hasOn2Clause;
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