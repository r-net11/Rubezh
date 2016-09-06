using System.ComponentModel.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System;
using System.ComponentModel;
using System.Drawing;
using Attribute = System.Attribute;

namespace ReportSystem.UI.Reports
{
	public partial class PassCardTemplateReport : XtraReport
	{
		public PassCardTemplateReport(Image image)
		{
			DXDisplayNameAttribute.UseResourceManager = true; 
			InitializeComponent();
			InitializeReport(image);
			RemoveBands();
			FilterComponentProperties += XtraReport_FilterComponentProperties;
			DesignerLoaded += OnDesignerLoaded;
		}

		private void OnDesignerLoaded(object sender, DesignerLoadedEventArgs designerLoadedEventArgs)
		{
			DXDisplayNameAttribute.UseResourceManager = true; 
			var detailBand = designerLoadedEventArgs.DesignerHost.GetService(typeof (IComponentChangeService)) as IComponentChangeService;

			if (detailBand == null) return;

			detailBand.ComponentChanged += DetailBandOnComponentChanged;
		}

		private void DetailBandOnComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			DXDisplayNameAttribute.UseResourceManager = true; 
			if (e == null || e.Member == null) return;

			var detailBand = e.Component as DetailBand;

			if (detailBand == null) return;

			if (e.Member.DisplayName == "Высота" && e.Member.Category == "Макет") //TODO: Add localized elements
				detailBand.HeightF = (float) e.OldValue;
			else if (e.Member.DisplayName == "Ширина страницы" && e.Member.Category == "Параметры страницы")
				detailBand.WidthF = (float) e.OldValue;
		}

		private void InitializeReport(Image image)
		{
			DXDisplayNameAttribute.UseResourceManager = true; 
			ReportUnit = ReportUnit.TenthsOfAMillimeter;
			DrawWatermark = true;

			//var dataSet = new Test();
		//	DataSource = dataSet;
		//	DataMember = dataSet.Tables[0].TableName;

			DesignerOptions.ShowExportWarnings = false;
			DesignerOptions.ShowPrintingWarnings = false;
			DesignerOptions.ShowDesignerHints = false;
			ReportPrintOptions.PrintOnEmptyDataSource = true;

			xrPictureBox1.SendToBack();
			xrPictureBox1.Sizing = ImageSizeMode.StretchImage;
			xrPictureBox1.Size = new Size(image.Width, image.Height);
			xrPictureBox1.DataBindings.Add("Image", DataSource, "Image"); //TODO: try to find workaround
			//xrPictureBox1.Visible = false;
			CreateDocument();
		}

		private void RemoveBands()
		{
			DXDisplayNameAttribute.UseResourceManager = true; 
			var band = Bands.GetBandByType(typeof(TopMarginBand));
			if (band != null)
				Bands.Remove(band);

			band = Bands.GetBandByType(typeof(BottomMarginBand));
			if (band != null)
				Bands.Remove(band);
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
		//private void PassCardTemplateReport_DesignerLoaded(object sender, DesignerLoadedEventArgs e)
		//{
		//	IDesignerHost host = Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

		//	if (host == null) return;

		//	foreach (IComponent c in host.Container.Components)
		//	{
		//		ComponentDesigner designer = host.GetDesigner(c) as ComponentDesigner;
		//		if (designer != null)
		//		{
		//			designer.ActionLists.Clear();
		//			designer.Verbs.Clear();
		//		}
		//	}
		//	IComponentChangeService serv = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
		//	if (serv == null) return;

		//	serv.ComponentAdded += (s2, e2) =>
		//	{
		//		ComponentDesigner designer = host.GetDesigner(e2.Component) as ComponentDesigner;
		//		if (designer != null)
		//		{
		//			designer.ActionLists.Clear();
		//			designer.Verbs.Clear();
		//		}
		//	};
		//}

		private void XtraReport_FilterComponentProperties(object sender, FilterComponentPropertiesEventArgs e)
		{
			//Hiding properties of all elements
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

			//Hiding properties of concrete elements.
			if (sender is PassCardTemplateReport && e.Component is XRControl &&
				((XRControl)e.Component).Name == "label1")
			{
				HideProperty("Name", e);
				HideProperty("DataBindings", e);
			}
		}

		static void HideProperty(String propertyName, FilterComponentPropertiesEventArgs filterComponentProperties)
		{
			var oldPropertyDescriptor = filterComponentProperties.Properties[propertyName] as PropertyDescriptor;
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
