using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrustructure.Plans.Designer
{
	public interface IElementPresenter
	{
		FrameworkElement Draw();
		List<string> ContextMenu { get; }
	}
}
