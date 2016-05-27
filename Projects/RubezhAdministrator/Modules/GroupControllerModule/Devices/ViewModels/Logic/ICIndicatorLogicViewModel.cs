using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class ICIndicatorLogicViewModel : BaseViewModel
	{
		public ICIndicatorLogicViewModel(GKBase gkBase, ICIndicatorLogic icIndicatorLogic)
		{
			Blink1ClausesGroup = new ClauseGroupViewModel(gkBase, icIndicatorLogic.Blink1ClausesGroup);
			Blink3ClausesGroup = new ClauseGroupViewModel(gkBase, icIndicatorLogic.Blink3ClausesGroup);
			OffClausesGroup = new ClauseGroupViewModel(gkBase, icIndicatorLogic.OffClausesGroup);
			UseOffCounterLogic = icIndicatorLogic.UseOffCounterLogic;
		}

		public ClauseGroupViewModel Blink1ClausesGroup { get; private set; }

		public ClauseGroupViewModel Blink3ClausesGroup { get; private set; }

		public ClauseGroupViewModel OffClausesGroup { get; private set; }

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

		public ICIndicatorLogic GetModel()
		{
			var result = new ICIndicatorLogic();
			result.Blink1ClausesGroup = Blink1ClausesGroup.GetClauseGroup();
			result.Blink3ClausesGroup = Blink3ClausesGroup.GetClauseGroup();
			result.OffClausesGroup = OffClausesGroup.GetClauseGroup();
			result.UseOffCounterLogic = UseOffCounterLogic;
			return result;
		}
	}
}
