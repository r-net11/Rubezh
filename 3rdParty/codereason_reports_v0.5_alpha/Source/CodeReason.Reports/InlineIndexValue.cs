/************************************************************************
 * Copyright: Hans Wolff
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please see the top level LICENSE.txt 
 *           file found in the root directory of CodeReason Reports.
 *
 * Author:   Hans Wolff
 *
 ************************************************************************/

using System.Windows;
using CodeReason.Reports.Interfaces;

namespace CodeReason.Reports
{
    /// <summary>
    /// Abstract class for fillable run values
    /// </summary>
    public abstract class InlineIndexValue : InlineHasValue, IIndexValue
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public virtual int Index
        {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
        }

		// Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(InlineIndexValue), new UIPropertyMetadata(null));
    }
}
