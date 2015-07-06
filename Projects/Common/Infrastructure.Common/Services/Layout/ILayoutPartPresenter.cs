﻿using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartPresenter
	{
		Guid UID { get; }

		string Name { get; }

		string IconSource { get; }

		BaseViewModel CreateContent(ILayoutProperties properties);
	}
}