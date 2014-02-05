using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartPresenter : LayoutPartPresenter
	{
		public UnknownLayoutPartPresenter(Guid uid)
		{
			UID = uid;
			Name = "Неизвестный элемент";
			Factory = (p) => null;
		}
	}
}