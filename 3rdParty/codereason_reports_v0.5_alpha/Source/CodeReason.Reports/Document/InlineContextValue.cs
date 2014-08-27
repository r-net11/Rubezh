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

using CodeReason.Reports.Interfaces;
using System.Windows;

namespace CodeReason.Reports.Document
{
    /// <summary>
    /// Contains a single report context value that is to be displayed on the report
    /// </summary>
	public class InlineContextValue : InlineHasValue, IAggregateValue, IInlineContextValue
    {
		/// <summary>
		/// Gets or sets the Type
		/// </summary>
		public virtual ReportContextValueType Type
		{
			get { return (ReportContextValueType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(ReportContextValueType), typeof(InlineContextValue), new UIPropertyMetadata(null));
		
		private string _aggregateGroup = null;
        /// <summary>
        /// Gets or sets the aggregate group
        /// </summary>
        public string AggregateGroup
        {
            get { return _aggregateGroup; }
            set { _aggregateGroup = value; }
        }
    }
}
