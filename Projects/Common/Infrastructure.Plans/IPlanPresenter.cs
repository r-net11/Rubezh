using Common;
using Infrastructure.Plans.Presenter;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;

namespace Infrastructure.Plans
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