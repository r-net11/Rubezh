using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace ReportsModule.Models
{
    public class ReportIndicationBlockTestModel
    {
        public string[] str { get; set; }
        public string[] numb { get; set; }
    }

    public class ReportIndicationBlockModel
    {
        public string Number { get; set; }
        public string PresentationName { get; set; }
        //public string HeaderTable { get; set; }
        //public List<Element> IndicationBlockTable { get; set; }
    }

    public class Element
    {
        public string Number { get; set; }
        public string PresentationName { get; set; }
    }

    public class ReportIndicationBlockModelTemp
    {
        protected ReportIndicationBlockModelTemp() { }

        public ReportIndicationBlockModelTemp(Device device)
        {
            if (device.Driver.DriverType != DriverType.IndicationBlock)
            {
                return;
            }
            Pages = new List<Page>();
            IndicationBlockNumber = device.DottedAddress;
            foreach (var pageDevice in device.Children)
            {
                Pages.Add(new Page(pageDevice));
            }
        }

        public string IndicationBlockNumber { get; set; }
        public List<Page> Pages { get; set; }
    }

    public class Page
    {
        private Page() { }

        public Page(Device device)
        {
            PageNumber = device.IntAddress;
            ElementsPage = new List<ElementPage>();
            foreach (var elementPage in device.Children)
            {
                ElementsPage.Add(new ElementPage(
                    elementPage.IntAddress,
                    elementPage.IndicatorLogic.Zones,
                    elementPage.IndicatorLogic.ToString()));
            }
        }

        public int PageNumber { get; set; }
        public List<ElementPage> ElementsPage { get; set; }
    }

    public class ElementPage
    {
        private ElementPage() { }

        public ElementPage(int No, List<ulong?> zonesNo, string presentationName)
        {
            this.No = No;
            ZonesNo = zonesNo;
            PresentationName = presentationName;
        }

        public int No { get; set; }
        public List<ulong?> ZonesNo { get; set; }

        string _presentationName;
        public string PresentationName
        {
            get
            {
                if (ZonesNo.Count == 1)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZonesNo[0]);
                    string presentationName = "";
                    if (zone != null)
                    {
                        presentationName = zone.PresentationName;
                    }
                    return ("Зоны: " + presentationName);
                }
                else
                {
                    return _presentationName;
                }
            }
            set
            {
                _presentationName = value;
            }
        }
    }
}