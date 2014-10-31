using System;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace AutomationModule.ViewModels
{
	public class ElementViewModel : BaseViewModel
	{
		public string Name { get; private set; }
		ElementBase ElementBase { get; set; }
		public Type ElementType { get; private set; }

		public ElementViewModel(ElementBase elementBase)
		{
			ElementBase = elementBase;
			ElementType = ElementBase.GetType();
		}

		public string PresentationName
		{
			get { return ElementBase.PresentationName; }
		}

		public Guid Uid
		{
			get { return ElementBase.UID; }
		}
	}
}
