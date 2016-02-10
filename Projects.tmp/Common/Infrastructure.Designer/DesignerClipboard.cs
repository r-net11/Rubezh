using System.Collections.Generic;
using Common;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Designer
{
	public class DesignerClipboard : Clipboard<List<ElementBase>>
	{
		public DesignerClipboard()
		{
			this.Buffer = new List<ElementBase>();
		}

		protected override void OnClear()
		{
			base.OnClear();
			this.Buffer = new List<ElementBase>();
		}
	}
}