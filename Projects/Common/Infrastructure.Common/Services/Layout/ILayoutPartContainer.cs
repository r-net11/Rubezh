using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
    public interface ILayoutPartContainer
    {
        Guid UID { get; }
        string Title { get; set; }
        string IconSource { get; set; }
		bool IsActive { get; set; }
        bool IsSelected { get; set; }
		void Activate();
		bool IsVisibleLayout { get; }

		LayoutPart LayoutPart { get; }
		ILayoutPartPresenter LayoutPartPresenter { get; }

		event EventHandler ActiveChanged;
		event EventHandler SelectedChanged;
    }
}
