using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;

namespace Infrustructure.Plans
{
	public interface IPlanExtension<T>
	{
		int Index { get; }
		string Title { get; }
		object TabPage { get; }
		bool ElementAdded(T plan, ElementBase element);
		bool ElementRemoved(T plan, ElementBase element);
		void RegisterDesignerItem(DesignerItem designerItem);
		IEnumerable<ElementBase> LoadPlan(T plan);
		IEnumerable<IInstrument> Instruments { get; }
		void ExtensionRegistered(CommonDesignerCanvas designerCanvas);
	}
}
