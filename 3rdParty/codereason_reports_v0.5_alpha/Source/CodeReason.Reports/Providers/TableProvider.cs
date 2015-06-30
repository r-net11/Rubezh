using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using CodeReason.Reports.Document;
using System.Windows;

namespace CodeReason.Reports.Providers
{
	internal class TableProvider
	{
		public TableRowGroup Clone(TableRowGroup rowGroup, bool returnRowGroup)
		{
			var group = rowGroup is TableRowGroupForDataTable && !returnRowGroup ? new TableRowGroupForDataTable() { TableName = ((TableRowGroupForDataTable)rowGroup).TableName } : new TableRowGroup();
			CopyProperties(rowGroup, group);
			foreach (var row in rowGroup.Rows)
				group.Rows.Add(Clone(row, false));
			return group;
		}
		public TableRow Clone(TableRow row, bool returnRow)
		{
			var newRow = row is TableRowForDataTable && !returnRow ? new TableRowForDataTable() { TableName = ((TableRowForDataTable)row).TableName } : new TableRow();
			CopyProperties(row, newRow);
			foreach (var cell in row.Cells)
				newRow.Cells.Add(Clone(cell));
			return newRow;
		}

		private TableCell Clone(TableCell cell)
		{
			var newCell = new TableCell();
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.ColumnSpanProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.ColumnSpanProperty, cell.ColumnSpan);
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.RowSpanProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.RowSpanProperty, cell.RowSpan);
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.PaddingProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.PaddingProperty, cell.Padding);
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.BorderBrushProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.BorderBrushProperty, cell.BorderBrush);
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.BorderThicknessProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.BorderThicknessProperty, cell.BorderThickness);
			if (DependencyPropertyHelper.GetValueSource(cell, TableCell.TextAlignmentProperty).BaseValueSource != BaseValueSource.Default)
				newCell.SetValue(TableCell.TextAlignmentProperty, cell.TextAlignment);
			CopyProperties(cell, newCell);
			foreach (var block in cell.Blocks)
				newCell.Blocks.Add(Clone(block));
			return newCell;
		}
		private Block Clone(Block block)
		{
			var paragraph = block as Paragraph;
			if (paragraph != null)
			{
				var newParagraph = new Paragraph();
				if (DependencyPropertyHelper.GetValueSource(paragraph, Paragraph.PaddingProperty).BaseValueSource != BaseValueSource.Default)
					newParagraph.SetValue(Paragraph.PaddingProperty, paragraph.Padding);
				if (DependencyPropertyHelper.GetValueSource(paragraph, Paragraph.MarginProperty).BaseValueSource != BaseValueSource.Default)
					newParagraph.SetValue(Paragraph.MarginProperty, paragraph.Margin);
				if (DependencyPropertyHelper.GetValueSource(paragraph, Paragraph.TextAlignmentProperty).BaseValueSource != BaseValueSource.Default)
					newParagraph.SetValue(Paragraph.TextAlignmentProperty, paragraph.TextAlignment);
				CopyProperties(paragraph, newParagraph);
				foreach (var inline in paragraph.Inlines)
					newParagraph.Inlines.Add(Clone(inline));
				return newParagraph;
			}
			throw new NotSupportedException();
		}
		private Inline Clone(Inline inline)
		{
			Inline newInline = null;
			if (inline is InlineTableCellIndexValue)
			{
				var inlineValue = (InlineTableCellIndexValue)inline;
				newInline = new InlineTableCellIndexValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					Index = inlineValue.Index,
					Format = inlineValue.Format,
					Value = inlineValue.Value,
				};
			}
			else if (inline is InlineTableCellValue)
			{
				var inlineValue = (InlineTableCellValue)inline;
				newInline = new InlineTableCellValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					PropertyName = inlineValue.PropertyName,
					Format = inlineValue.Format,
					Value = inlineValue.Value,
				};
			}
			else if (inline is InlineAggregateValue)
			{
				var inlineValue = (InlineAggregateValue)inline;
				newInline = new InlineAggregateValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					EmptyValue = inlineValue.EmptyValue,
					ErrorValue = inlineValue.ErrorValue,
					Format = inlineValue.Format,
					Value = inlineValue.Value,
				};
			}
			else if (inline is InlineContextValue)
			{
				var inlineValue = (InlineContextValue)inline;
				newInline = new InlineContextValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					Type = inlineValue.Type,
					Format = inlineValue.Format,
					Value = inlineValue.Value,
				};
			}
			else if (inline is InlineDocumentValue)
			{
				var inlineValue = (InlineDocumentValue)inline;
				newInline = new InlineDocumentValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					PropertyName = inlineValue.PropertyName,
					Format = inlineValue.Format,
					Value = inlineValue.Value,
				};
			}
			else if (inline is Run)
			{
				var run = (Run)inline;
				newInline = new Run()
				{
					Text = run.Text,
				};
			}
			else if (inline is LineBreak)
				newInline = new LineBreak();
			else if (inline is Bold)
			{
				var bold = new Bold();
				foreach (var inl in ((Bold)inline).Inlines)
					bold.Inlines.Add(Clone(inl));
				newInline = bold;
			}
			if (newInline != null)
			{
				CopyProperties(inline, newInline);
				return newInline;
			}
			throw new NotSupportedException();
		}
		private void CopyProperties(TextElement source, TextElement target)
		{
			if (DependencyPropertyHelper.GetValueSource(source, Run.FontSizeProperty).BaseValueSource != BaseValueSource.Default)
				target.SetValue(Run.FontSizeProperty, source.FontSize);
			if (DependencyPropertyHelper.GetValueSource(source, Run.FontWeightProperty).BaseValueSource != BaseValueSource.Default)
				target.SetValue(Run.FontWeightProperty, source.FontWeight);
		}
	}
}
