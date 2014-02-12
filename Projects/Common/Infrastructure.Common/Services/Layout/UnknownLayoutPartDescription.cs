﻿using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartDescription : LayoutPartDescription
	{
		public UnknownLayoutPartDescription(Guid uid)
			: base(uid, 1, "Неизвестный элемент", null)
		{
		}
	}
}
