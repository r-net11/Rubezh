using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartDescription : LayoutPartDescription
	{
		public UnknownLayoutPartDescription(Guid uid)
		{
			UID = uid;
			Name = "Неизвестный элемент";
		}
	}
}
