﻿using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.CodeParser;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Attribute = System.Attribute;

namespace ReportSystem.Templates
{
	public partial class PassCardTemplateReport : XtraReport
	{
		public PassCardTemplateReport()
		{
			InitializeComponent();
			this.DrawWatermark = true;
			var dataSet = new EmployeeDataSet();
			this.DataSource = dataSet;
			this.DataMember = dataSet.Tables[0].TableName;

			ReportPrintOptions.PrintOnEmptyDataSource = true;

			var band = Bands.GetBandByType(typeof (TopMarginBand));
			if(band != null) Bands.Remove(band);

			band = Bands.GetBandByType(typeof (BottomMarginBand));
			if(band != null) Bands.Remove(band);
			XtraReport.FilterComponentProperties += XtraReport_FilterComponentProperties;
		}
		public PassCardTemplateReport(Image image) : this()
		{
			Watermark.Image = image;
			this.CreateDocument();
		}

		private void TopMargin_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			e.Cancel = true;
		}

		private void BottomMargin_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			e.Cancel = true;
		}


		//Удаление смарт тегов отчета и элементов дизайнера пропусков.
		private void PassCardTemplateReport_DesignerLoaded(object sender, DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs e)
		{
			IDesignerHost host = Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

			if (host == null) return;

			foreach (IComponent c in host.Container.Components)
			{
				ComponentDesigner designer = host.GetDesigner(c) as ComponentDesigner;
				if (designer != null)
				{
					designer.ActionLists.Clear();
					designer.Verbs.Clear();
				}
			}
			IComponentChangeService serv = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if (serv == null) return;

			serv.ComponentAdded += (s2, e2) =>
			{
				ComponentDesigner designer = host.GetDesigner(e2.Component) as ComponentDesigner;
				if (designer != null)
				{
					designer.ActionLists.Clear();
					designer.Verbs.Clear();
				}
			};
		}

		void XtraReport_FilterComponentProperties(object sender,
		FilterComponentPropertiesEventArgs e)
		{
			// The following line hides the Scripts property for all report elements.
			HideProperty("Scripts", e);
			HideProperty("Watermark", e);
			HideProperty("DrawWatermark", e);
			HideProperty("FormattingRuleLinks", e);
			HideProperty("FormattingRuleSheet", e);
			HideProperty("FormattingRules", e);
			HideProperty("StyleSheetPath", e);
			HideProperty("StyleSheet", e);
			HideProperty("DataAdapter", e);
			HideProperty("CalculatedFields", e);
			HideProperty("DataSource", e);
			HideProperty("XmlDataPath", e);
			HideProperty("Tag", e);
			HideProperty("FilterString", e);
			HideProperty("DataMember", e);
			HideProperty("DesignerOptions", e);
			HideProperty("Name", e);
			HideProperty("DataSourceSchema", e);
			HideProperty("Bookmark", e);
			HideProperty("Parameters", e);
			HideProperty("RequestParameters", e);
			HideProperty("PageHeight", e);
			HideProperty("DefaultPrinterSettingsUsing", e);
			HideProperty("Margins", e);
			HideProperty("PaperName", e);
			HideProperty("PrinterName", e);
			HideProperty("PaperKind", e);
			HideProperty("PageWidth", e);
			HideProperty("ReportPrintOptions", e);
			HideProperty("ShowPrintMarginsWarning", e);
			HideProperty("ShowPrintStatusDialog", e);
			HideProperty("DisplayName", e);
			HideProperty("ExportOptions", e);
			HideProperty("ScriptLanguage", e);
			HideProperty("Bands", e);
			HideProperty("Extensions", e);
			// The following code hides the ReportSource property,
			// for subreports to always contain only the pre-defined reports.
			if (e.Component is XRSubreport)
			{
				HideProperty("ReportSource", e);
				HideProperty("ReportSourceUrl", e);
			}

			// The following code hides some properties for a specific report element.
			if (sender is PassCardTemplateReport && e.Component is XRControl &&
				((XRControl)e.Component).Name == "label1")
			{
				HideProperty("Name", e);
				HideProperty("DataBindings", e);
			}
		}

		static void HideProperty(String propertyName,
			FilterComponentPropertiesEventArgs filterComponentProperties)
		{
			PropertyDescriptor oldPropertyDescriptor =
				filterComponentProperties.Properties[propertyName] as PropertyDescriptor;
			if (oldPropertyDescriptor != null)
			{
				// Substitute the current property descriptor
				// with a custom one with the BrowsableAttribute.No attribute.
				filterComponentProperties.Properties[propertyName] = TypeDescriptor.CreateProperty(
					oldPropertyDescriptor.ComponentType,
					oldPropertyDescriptor,
					new Attribute[] { BrowsableAttribute.No });
			}
			else
			{
				// If the property descriptor can not be substituted,
				// remove it from the Properties collection.
				filterComponentProperties.Properties.Remove(propertyName);
			}
		}

	}
}