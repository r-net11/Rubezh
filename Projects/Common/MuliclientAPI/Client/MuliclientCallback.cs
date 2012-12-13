using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MuliclientAPI
{
	public class MuliclientCallback : IMuliclientCallback
	{
		public void Show()
		{
			if (ShowEvent != null)
				ShowEvent();
		}

		public void Hide()
		{
			if (HideEvent != null)
				HideEvent();
		}

		public static event Action ShowEvent;
		public static event Action HideEvent;
	}
}