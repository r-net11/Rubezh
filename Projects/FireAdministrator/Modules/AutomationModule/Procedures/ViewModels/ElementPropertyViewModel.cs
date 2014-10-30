using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ElementPropertyViewModel : BaseViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ElementProperty ElementProperty { get; set; }

		public ElementPropertyViewModel(ElementProperty elementProperty)
		{
			ElementProperty = elementProperty;
		}

		ExplicitType ElementPropertyTypeToExplicitType(ElementPropertyType elementPropertyType)
		{
			if ((elementPropertyType == ElementPropertyType.Height) || (elementPropertyType == ElementPropertyType.Width))
				return ExplicitType.Integer;
			return ExplicitType.Integer;
		}
	}
}
