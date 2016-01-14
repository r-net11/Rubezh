using Common;
using RubezhAPI.Models;

namespace PlansModule
{
	public class PlansClipboard : Clipboard<Plan>
	{
		protected override void OnClear()
		{
			base.OnClear();
			this.Buffer = null;
		}
	}
}