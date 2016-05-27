using Common;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Presenter;
using System;
using System.Collections.Generic;

namespace Infrustructure.Plans
{
	public interface IPlanPresenter<TPlan, TState>
	{
		void SubscribeStateChanged(TPlan plan, Action callBack);

		NamedStateClass GetNamedStateClass(TPlan plan);

		IEnumerable<ElementBase> LoadPlan(TPlan plan);

		void RegisterPresenterItem(PresenterItem presenterItem);

		void ExtensionAttached();
	}
}