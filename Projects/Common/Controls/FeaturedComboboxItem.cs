using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	/// <summary>
	/// Extends the <see cref="ComboBoxItem"/> class.
	/// </summary>
	[Browsable(false)]
	public class FeaturedComboboxItem : ComboBoxItem
	{
		/// <summary>
		/// Gets or sets a <see cref="bool"/> value that indicates if this item is highlighted.
		/// </summary>
		public new bool IsHighlighted
		{
			get { return base.IsHighlighted; }
			set { base.IsHighlighted = value; }
		}

		static FeaturedComboboxItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FeaturedComboboxItem), new FrameworkPropertyMetadata(typeof(FeaturedComboboxItem)));
		}
	}
}
