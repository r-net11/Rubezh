using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;

namespace Infrastructure.Plans
{
	public interface IPlanExtension<T>
	{
		int Index { get; }
		string Title { get; }
		bool ElementAdded(T plan, ElementBase element);
		bool ElementRemoved(T plan, ElementBase element);
		void RegisterDesignerItem(DesignerItem designerItem);
		IEnumerable<ElementBase> LoadPlan(T plan);
		IEnumerable<IInstrument> Instruments { get; }
		void ExtensionRegistered(CommonDesignerCanvas designerCanvas);
		void ExtensionAttached();
		IEnumerable<ElementError> Validate();
	}
}