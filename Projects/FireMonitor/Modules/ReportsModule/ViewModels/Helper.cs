using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Documents;

namespace ReportsModule
{
    public static class Helper
    {
        public static void AddDataColumns<T>(DataTable dataTable)
        {
            var propertyInfos = typeof(T).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }
        }
        public static Size Subtract(this Size s1, Size s2)
        {
            return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        }
    }

    public static class BindableExtender
    {
        public static Table GetBindableTable(DependencyObject obj)
        {
            return (Table)obj.GetValue(BindableTableProperty);
        }

        public static void SetBindableTable(DependencyObject obj, Table tbl)
        {
            obj.SetValue(BindableTableProperty, tbl);
        }

        public static string GetBindableText(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableTextProperty);

        }

        public static void SetBindableText(DependencyObject obj, string value)
        {
            obj.SetValue(BindableTextProperty, value);
        }

        public static readonly DependencyProperty BindableTextProperty = DependencyProperty.RegisterAttached("BindableText",
                typeof(string), typeof(BindableExtender), new UIPropertyMetadata(null, BindableProperty_PropertyChanged));

        public static readonly DependencyProperty BindableTableProperty = DependencyProperty.RegisterAttached("BindableTable",
            typeof(Table), typeof(BindableExtender), new UIPropertyMetadata(null,
                    BindableProperty_PropertyChanged));

        private static void BindableProperty_PropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Run)
            {
                ((Run)dependencyObject).Text = (string)e.NewValue;
            }
            if (dependencyObject is FlowDocument)
            {
                ((FlowDocument)dependencyObject).Blocks.Clear();
                if (e.NewValue != null)
                {
                    ((FlowDocument)dependencyObject).Blocks.Add((Block)e.NewValue);
                }
            }
        }

        private static Table BuildTable()
        {
            Table tbl = new Table();
            TableRowGroup rowGrp = new TableRowGroup();
            tbl.RowGroups.Add(rowGrp);
            TableRow row = new TableRow();
            rowGrp.Rows.Add(row);
            row.Cells.Add(new TableCell(new Paragraph(new Run("DataLabel:"))));
            //Paragraph pg = new Paragraph();
            //pg.Inlines.Add(new Run("Some data"));
            //row.Cells.Add(pg);
            return tbl;
        }
    }
}
