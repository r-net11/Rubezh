using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		event EventHandler ActiveChanged;
		event EventHandler SelectedChanged;
    }
}
