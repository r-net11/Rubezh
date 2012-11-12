using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
		public PresenterItem(ElementBase element)
			: base(element)
		{
			IsVisibleLayout = true;
			IsSelectableLayout = false;
		}

		protected override void CreateContextMenu()
		{
		}
	}
}
