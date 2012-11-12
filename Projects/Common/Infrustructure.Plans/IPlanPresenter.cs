using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace Infrustructure.Plans
{
	public interface IPlanPresenter<T>
	{
		IEnumerable<ElementBase> LoadPlan(T plan);
		void RegisterPresenterItem(PresenterItem presenterItem);
	}
}
