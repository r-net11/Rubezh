using Common;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;

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