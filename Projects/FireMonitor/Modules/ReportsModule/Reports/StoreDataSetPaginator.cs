using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Data;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using Infrastructure.Common;
using Common;
using FiresecClient;

namespace ReportsModule.Reports
{
    public static class TestReport
    {
        static List<DeviceList> DevicesList;

        public static DataTable CreateDataTable()
        {

            Initialize();

            var dataTable = new DataTable("DevicesList");
            dataTable.Columns.Add("Column1");
            dataTable.Columns.Add("Column2");
            dataTable.Columns.Add("Column3");
            foreach (var device in DevicesList)
            {
                dataTable.Rows.Add(device.Type, device.Address, device.ZoneName);
            }
            return dataTable;
        }

        static void Initialize()
        {
            DevicesList = new List<DeviceList>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                string type = "";
                string address = "";
                string zonePresentationName = "";
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    zonePresentationName = "";
                    var a = device.ZoneLogic;
                    var b = device.IndicatorLogic;
                    type = device.Driver.ShortName;
                    var category = device.Driver.Category;
                    address = device.DottedAddress;
                    if (device.Driver.IsZoneDevice)
                    {

                        if (FiresecManager.DeviceConfiguration.Zones.IsNotNullOrEmpty())
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                            if (zone != null)
                            {
                                zonePresentationName = zone.PresentationName;
                            }
                        }
                    }

                    DevicesList.Add(new DeviceList()
                    {
                        Type = type,
                        Address = address,
                        ZoneName = zonePresentationName
                    });
                }
            }
        }
    }

    public class StoreDataSetPaginator : DocumentPaginator
    {
        private DataTable dt;
        private Typeface typeface;
        private double fontSize;
        private double margin;
        private Size pageSize;
        public override Size PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                PaginateData();
            }
        }
        public StoreDataSetPaginator(DataTable dt, Typeface typeface,
        double fontSize, double margin, Size pageSize)
        {
            this.dt = dt;
            this.typeface = typeface;
            this.fontSize = fontSize;
            this.margin = margin;
            this.pageSize = pageSize;
            PaginateData();
        }

        private int rowsPerPage;
        private int pageCount;
        private void PaginateData()
        {
            FormattedText text = GetFormattedText("A");
            rowsPerPage = (int)((pageSize.Height - margin * 2) / text.Height);
            rowsPerPage -= 1;
            pageCount = (int)Math.Ceiling((double)dt.Rows.Count / rowsPerPage);
        }

        private FormattedText GetFormattedText(string text)
        {
            return GetFormattedText(text, typeface);
        }
        private FormattedText GetFormattedText(string text, Typeface typeface)
        {
            return new FormattedText(
            text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            typeface, fontSize, Brushes.Black);
        }



        public override bool IsPageCountValid
        {
            get { return true; }
        }
        public override int PageCount
        {
            get { return pageCount; }
        }
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            FormattedText text = GetFormattedText("A");
            double col1_X = margin;
            double col2_X = col1_X + text.Width * 15;
            double col3_X = col2_X + text.Width * 15;

            int minRow = pageNumber * rowsPerPage;
            int maxRow = minRow + rowsPerPage;
            DrawingVisual visual = new DrawingVisual();
            Point point = new Point(margin, margin);
            using (DrawingContext dc = visual.RenderOpen())
            {
                Typeface columnHeaderTypeface = new Typeface(
                typeface.FontFamily, FontStyles.Normal, FontWeights.Bold,
                FontStretches.Normal);
                point.X = col1_X;
                text = GetFormattedText("Column1", columnHeaderTypeface);
                dc.DrawText(text, point);
                text = GetFormattedText("Column2", columnHeaderTypeface);
                point.X = col2_X;
                dc.DrawText(text, point);
                text = GetFormattedText("Column3", columnHeaderTypeface);
                point.X = col3_X;
                dc.DrawText(text, point);

                dc.DrawLine(new Pen(Brushes.Black, 2),
                new Point(margin, margin + text.Height),
                new Point(pageSize.Width - margin, margin + text.Height));
                point.Y += text.Height;

                for (int i = minRow; i < maxRow; i++)
                {
                    if (i > (dt.Rows.Count - 1)) break;
                    point.X = col1_X;
                    text = GetFormattedText(dt.Rows[i]["Column1"].ToString());
                    dc.DrawText(text, point);

                    text = GetFormattedText(dt.Rows[i]["Column2"].ToString());
                    point.X = col2_X;
                    dc.DrawText(text, point);
                    text = GetFormattedText(dt.Rows[i]["Column3"].ToString());
                    point.X = col3_X;
                    dc.DrawText(text, point);
                    point.Y += text.Height;
                }
            }
            return new DocumentPage(visual, pageSize, new Rect(pageSize), new Rect(pageSize));
        }

        //public override DocumentPage GetPage(int pageNumber)
        //{
        //    DocumentPage page = flowDocumentPaginator.GetPage(pageNumber);
        //    ContainerVisual newVisual = new ContainerVisual();
        //    newVisual.Children.Add(page.Visual);
        //    DrawingVisual header = new DrawingVisual();
        //    using (DrawingContext dc = header.RenderOpen())
        //    {
        //        Typeface typeface = new Typeface("Times New Roman");
        //        FormattedText text = new FormattedText("Page " + (pageNumber + 1).ToString(), CultureInfo.CurrentCulture,
        //        FlowDirection.LeftToRight, typeface, 14, Brushes.Black);
        //        dc.DrawText(text, new Point(96 * 0.25, 96 * 0.25));
        //    }
        //    newVisual.Children.Add(header);
        //    DocumentPage newPage = new DocumentPage(newVisual);
        //    return newPage;
        //}

    }
}
