using Infrastructure.Common.Windows.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	/// <summary>
	/// View Model for Primitive's Tooltips.
	/// </summary>
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
