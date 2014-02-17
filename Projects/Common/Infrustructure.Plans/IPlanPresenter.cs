using System;
using System.Collections.Generic;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace Infrustructure.Plans
{
	public interface IPlanPresenter<TPlan, TState>
	{
		void SubscribeStateChanged(TPlan plan, Action callBack);
		TState GetState(TPlan plan);

		IEnumerable<ElementBase> LoadPlan(TPlan plan);
		void RegisterPresenterItem(PresenterItem presenterItem);
		void ExtensionAttached();
	}
}