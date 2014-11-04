using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class LayoutTextBoxPartViewModel : BaseViewModel, ILayoutPartControl
	{
		private string _tetx;
		public string Text
		{
			get { return _tetx; }
			set
			{
				_tetx = value;
				OnPropertyChanged(() => Text);
			}
		}

		#region ILayoutPartControl Members

		public object GetProperty(LayoutPartPropertyName property)
		{
			return property == LayoutPartPropertyName.Text ? Text : null;
		}

		public void SetProperty(LayoutPartPropertyName property, object value)
		{
			if (property == LayoutPartPropertyName.Text)
				Text = value.ToString();
		}

		#endregion
	}
}
