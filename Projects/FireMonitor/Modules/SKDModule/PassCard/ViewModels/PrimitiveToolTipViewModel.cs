using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.PassCard.ViewModels
{
	public class PrimitiveToolTipViewModel : BaseViewModel
	{
		/// <summary>
		/// Sets/retrieves the Name of the Primitive.
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set
			{
				this.name = value;
				base.OnPropertyChanged(() => this.Name);
			}
		}
		private string name = string.Empty;
	}
}
