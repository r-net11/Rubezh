using System;
using System.Collections.Generic;
using System.Windows.Media;
using Common;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Client.Images;
using Infrastructure.Client.Layout.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class LayoutPartTimeViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartTimeProperties _properties;
		public LayoutPartTimeViewModel(LayoutPartTimeProperties properties)
		{
			_properties = properties ?? new LayoutPartTimeProperties();
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyTimePageViewModel(this);
			}
		}
	}
}