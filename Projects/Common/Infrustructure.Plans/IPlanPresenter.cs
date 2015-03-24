using System;
using System.Collections.Generic;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;
using Common;

namespace Infrustructure.Plans
{
	public interface IPlanPresenter<TPlan, TState>
	{
		void SubscribeStateChanged(TPlan plan, Action callBack);
		StateTypeName<TState> GetStateTypeName(TPlan plan);

		IEnumerable<ElementBase> LoadPlan(TPlan plan);
		void RegisterPresenterItem(PresenterItem presenterItem);
		void ExtensionAttached();
	}
}