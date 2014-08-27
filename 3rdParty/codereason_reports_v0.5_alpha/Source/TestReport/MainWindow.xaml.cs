using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using System.Collections.Generic;

namespace TestReport
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private bool _firstActivated = true;

		private void Window_Activated(object sender, EventArgs e)
		{
			//if (!_firstActivated) return;
			//_firstActivated = false;

			try
			{
				ReportDocument reportDocument = new ReportDocument();

				StreamReader reader = new StreamReader(new FileStream(@"..\..\..\..\..\..\Projects\FireMonitor\Modules\SKDModule\Reports\T13.xaml", FileMode.Open, FileAccess.Read));
				//StreamReader reader = new StreamReader(new FileStream(@"..\..\Templates\Test.xaml", FileMode.Open, FileAccess.Read));
				reportDocument.XamlData = reader.ReadToEnd();
				reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
				reader.Close();

				var table = new DataTable();
				using (var sr = new StringReader(tableData))
					table.ReadXml(sr);

				//const int RecordOnPage = 3;
				//var data = new List<ReportData>();
				//ReportData reportData = null;
				//int countOnPage = RecordOnPage;
				//for (int i = 0; i < table.Rows.Count; i++)
				//{
				//    if (reportData == null)
				//    {
				//        reportData = new ReportData();
				//        SetDocumentValues(reportData);
				//        reportData.DataTables.Add(table.Clone());
				//        if (data.Count == 0)
				//        {
				//            reportData.Groups.Add("Header");
				//            countOnPage = RecordOnPage - 1;
				//        }
				//        else if (i + RecordOnPage == table.Rows.Count)
				//            countOnPage = RecordOnPage - 1;
				//        else if (i + RecordOnPage > table.Rows.Count)
				//            reportData.Groups.Add("Footer");
				//        else
				//            countOnPage = RecordOnPage;
				//        data.Add(reportData);
				//    }
				//    reportData.DataTables[0].Rows.Add(table.Rows[i].ItemArray);
				//    if (reportData.DataTables[0].Rows.Count == countOnPage)
				//        reportData = null;
				//}
				ReportData data = new ReportData();
				data.Groups.Add("Header");
				data.Groups.Add("Footer");
				SetDocumentValues(data);
				data.DataTables.Add(table);

				DateTime dateTimeStart = DateTime.Now; // start time measure here
				XpsDocument xps = reportDocument.CreateXpsDocument(data);
				documentViewer.Document = xps.GetFixedDocumentSequence();

				// show the elapsed time in window title
				Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
			}
			catch (Exception ex)
			{
				// show exception
				MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
			}
		}

		private void SetDocumentValues(ReportData data)
		{
			// set constant document values
			data.ReportDocumentValues.Add("PrintDate", new DateTime(2015, 9, 29));
			data.ReportDocumentValues.Add("StartDate", new DateTime(2015, 9, 30));
			data.ReportDocumentValues.Add("EndDate", new DateTime(2015, 9, 1));

			data.ReportDocumentValues.Add("FillName", "Иванов И.С.");
			data.ReportDocumentValues.Add("HRName", "Петров П.Г.");
			data.ReportDocumentValues.Add("LeadName", "Сидоров Ф.П.");

			data.ReportDocumentValues.Add("FillPosition", "Заведующий канцелярией");
			data.ReportDocumentValues.Add("HRPosition", "Заведующий канцелярией");
			data.ReportDocumentValues.Add("LeadPosition", "Начальник кадровой службы");

			data.ReportDocumentValues.Add("Organization", "ООО \"Сатурн-Экспорт\"");
			data.ReportDocumentValues.Add("Department", "концелярия");
		}

		private string tableData = @"<?xml version=""1.0"" standalone=""yes""?>
<NewDataSet>
  <xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""Data"" msdata:UseCurrentLocale=""true"">
      <xs:complexType>
        <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
          <xs:element name=""Data"">
            <xs:complexType>
              <xs:sequence>
                <xs:element name=""No"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""EmploueeFIO"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""TabelNo"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""FirstHalfDays"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""SecondHalfDays"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""TotalDays"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""FirstHalfHours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""SecondHalfHours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""TotalHours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day1_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day1_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day2_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day2_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day3_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day3_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day4_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day4_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day5_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day5_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day6_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day6_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day7_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day7_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day8_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day8_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day9_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day9_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day10_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day10_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day11_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day11_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day12_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day12_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day13_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day13_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day14_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day14_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day15_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day15_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day16_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day16_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day17_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day17_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day18_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day18_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day19_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day19_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day20_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day20_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day21_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day21_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day22_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day22_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day23_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day23_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day24_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day24_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day25_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day25_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day26_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day26_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day27_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day27_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day28_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day28_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day29_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day29_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day30_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day30_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day31_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""Day31_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason1_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason1_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason2_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason2_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason3_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason3_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason4_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason4_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason5_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason5_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason6_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason6_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason7_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason7_Hours"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason8_Code"" type=""xs:string"" minOccurs=""0"" />
                <xs:element name=""MissReason8_Hours"" type=""xs:string"" minOccurs=""0"" />
              </xs:sequence>
            </xs:complexType>
          </xs:element>
        </xs:choice>
      </xs:complexType>
    </xs:element>
  </xs:schema>
  <Data>
    <No>1</No>
    <EmploueeFIO> Сидорович</EmploueeFIO>
    <TabelNo>668</TabelNo>
    <FirstHalfDays>19</FirstHalfDays>
    <SecondHalfDays>1</SecondHalfDays>
    <TotalDays>19</TotalDays>
    <FirstHalfHours>124</FirstHalfHours>
    <SecondHalfHours>5</SecondHalfHours>
    <TotalHours>165</TotalHours>
    <Day1_Code />
    <Day1_Hours>3</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>5</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>8</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>4</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>4</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>8</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>13</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>2</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>6</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>23</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>14</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>11</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>20</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>20</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>2</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>4</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>19</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>23</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>0</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>8</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>9</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>16</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>19</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>15</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>21</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>10</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>15</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>14</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>7</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>5</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>16</Day31_Hours>
    <MissReason1_Code>Т1</MissReason1_Code>
    <MissReason1_Hours>21</MissReason1_Hours>
    <MissReason2_Code>Т2</MissReason2_Code>
    <MissReason2_Hours>11</MissReason2_Hours>
    <MissReason3_Code>Т3</MissReason3_Code>
    <MissReason3_Hours>22</MissReason3_Hours>
    <MissReason4_Code>Т4</MissReason4_Code>
    <MissReason4_Hours>7</MissReason4_Hours>
    <MissReason5_Code>Т5</MissReason5_Code>
    <MissReason5_Hours>8</MissReason5_Hours>
    <MissReason6_Code>Т6</MissReason6_Code>
    <MissReason6_Hours>5</MissReason6_Hours>
    <MissReason7_Code>Т7</MissReason7_Code>
    <MissReason7_Hours>13</MissReason7_Hours>
    <MissReason8_Code>Т8</MissReason8_Code>
    <MissReason8_Hours>5</MissReason8_Hours>
  </Data>
  <Data>
    <No>2</No>
    <EmploueeFIO> Прохорович</EmploueeFIO>
    <TabelNo>163</TabelNo>
    <FirstHalfDays>697</FirstHalfDays>
    <SecondHalfDays>30</SecondHalfDays>
    <TotalDays>741</TotalDays>
    <FirstHalfHours>18</FirstHalfHours>
    <SecondHalfHours>1</SecondHalfHours>
    <TotalHours>17</TotalHours>
    <Day1_Code />
    <Day1_Hours>22</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>6</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>13</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>22</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>4</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>4</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>19</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>20</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>10</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>10</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>10</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>17</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>6</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>17</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>17</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>22</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>18</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>0</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>6</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>16</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>0</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>22</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>12</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>18</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>22</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>6</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>17</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>20</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>16</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>8</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>22</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>0</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>13</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>4</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>11</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>9</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>0</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>8</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>1</MissReason8_Hours>
  </Data>
  <Data>
    <No>3</No>
    <EmploueeFIO> Захарович</EmploueeFIO>
    <TabelNo>522</TabelNo>
    <FirstHalfDays>93</FirstHalfDays>
    <SecondHalfDays>641</SecondHalfDays>
    <TotalDays>497</TotalDays>
    <FirstHalfHours>20</FirstHalfHours>
    <SecondHalfHours>22</SecondHalfHours>
    <TotalHours>12</TotalHours>
    <Day1_Code />
    <Day1_Hours>6</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>12</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>10</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>17</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>5</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>16</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>11</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>8</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>17</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>0</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>17</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>19</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>0</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>22</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>11</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>22</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>4</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>1</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>11</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>9</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>3</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>2</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>5</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>8</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>21</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>9</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>23</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>18</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>14</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>9</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>11</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>16</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>20</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>10</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>23</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>5</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>10</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>22</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>2</MissReason8_Hours>
  </Data>
  <Data>
    <No>4</No>
    <EmploueeFIO> Иванович</EmploueeFIO>
    <TabelNo>278</TabelNo>
    <FirstHalfDays>555</FirstHalfDays>
    <SecondHalfDays>212</SecondHalfDays>
    <TotalDays>473</TotalDays>
    <FirstHalfHours>11</FirstHalfHours>
    <SecondHalfHours>15</SecondHalfHours>
    <TotalHours>5</TotalHours>
    <Day1_Code />
    <Day1_Hours>8</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>3</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>0</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>3</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>10</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>9</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>8</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>12</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>8</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>4</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>6</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>2</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>19</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>20</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>16</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>17</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>18</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>8</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>12</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>1</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>23</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>14</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>22</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>6</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>12</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>5</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>23</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>5</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>20</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>22</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>15</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>18</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>16</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>16</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>1</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>20</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>17</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>10</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>22</MissReason8_Hours>
  </Data>
  <Data>
    <No>5</No>
    <EmploueeFIO> Перович</EmploueeFIO>
    <TabelNo>56</TabelNo>
    <FirstHalfDays>309</FirstHalfDays>
    <SecondHalfDays>313</SecondHalfDays>
    <TotalDays>47</TotalDays>
    <FirstHalfHours>19</FirstHalfHours>
    <SecondHalfHours>22</SecondHalfHours>
    <TotalHours>7</TotalHours>
    <Day1_Code />
    <Day1_Hours>2</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>6</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>13</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>19</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>10</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>3</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>22</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>13</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>6</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>18</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>4</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>18</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>15</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>5</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>10</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>3</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>16</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>10</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>13</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>12</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>9</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>3</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>18</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>20</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>23</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>7</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>9</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>11</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>18</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>16</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>13</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>3</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>7</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>4</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>3</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>16</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>15</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>0</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>11</MissReason8_Hours>
  </Data>
  <Data>
    <No>6</No>
    <EmploueeFIO> Иванович</EmploueeFIO>
    <TabelNo>519</TabelNo>
    <FirstHalfDays>826</FirstHalfDays>
    <SecondHalfDays>126</SecondHalfDays>
    <TotalDays>424</TotalDays>
    <FirstHalfHours>7</FirstHalfHours>
    <SecondHalfHours>16</SecondHalfHours>
    <TotalHours>22</TotalHours>
    <Day1_Code />
    <Day1_Hours>19</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>17</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>14</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>3</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>15</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>14</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>6</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>10</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>21</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>22</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>11</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>17</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>20</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>3</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>17</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>11</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>19</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>14</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>4</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>0</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>11</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>10</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>2</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>18</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>12</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>15</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>22</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>0</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>16</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>6</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>17</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>11</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>4</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>1</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>11</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>16</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>21</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>14</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>0</MissReason8_Hours>
  </Data>
  <Data>
    <No>7</No>
    <EmploueeFIO> Сергеевич</EmploueeFIO>
    <TabelNo>968</TabelNo>
    <FirstHalfDays>918</FirstHalfDays>
    <SecondHalfDays>859</SecondHalfDays>
    <TotalDays>290</TotalDays>
    <FirstHalfHours>4</FirstHalfHours>
    <SecondHalfHours>4</SecondHalfHours>
    <TotalHours>22</TotalHours>
    <Day1_Code />
    <Day1_Hours>4</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>7</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>18</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>8</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>7</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>19</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>2</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>15</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>10</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>8</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>15</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>20</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>15</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>23</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>15</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>5</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>18</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>15</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>4</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>0</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>13</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>18</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>15</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>1</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>14</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>4</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>13</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>5</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>2</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>15</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>4</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>21</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>13</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>13</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>11</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>15</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>6</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>8</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>23</MissReason8_Hours>
  </Data>
  <Data>
    <No>8</No>
    <EmploueeFIO> Сидорович</EmploueeFIO>
    <TabelNo>944</TabelNo>
    <FirstHalfDays>371</FirstHalfDays>
    <SecondHalfDays>520</SecondHalfDays>
    <TotalDays>615</TotalDays>
    <FirstHalfHours>3</FirstHalfHours>
    <SecondHalfHours>1</SecondHalfHours>
    <TotalHours>0</TotalHours>
    <Day1_Code />
    <Day1_Hours>23</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>1</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>6</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>16</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>1</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>15</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>23</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>4</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>9</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>9</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>15</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>17</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>18</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>2</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>6</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>20</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>12</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>18</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>4</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>17</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>7</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>1</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>11</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>0</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>22</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>9</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>15</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>6</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>15</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>0</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>4</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>12</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>0</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>13</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>5</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>11</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>22</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>10</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>13</MissReason8_Hours>
  </Data>
  <Data>
    <No>9</No>
    <EmploueeFIO> Дайтаро</EmploueeFIO>
    <TabelNo>524</TabelNo>
    <FirstHalfDays>959</FirstHalfDays>
    <SecondHalfDays>383</SecondHalfDays>
    <TotalDays>193</TotalDays>
    <FirstHalfHours>1</FirstHalfHours>
    <SecondHalfHours>23</SecondHalfHours>
    <TotalHours>12</TotalHours>
    <Day1_Code />
    <Day1_Hours>14</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>20</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>15</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>11</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>23</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>19</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>22</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>7</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>17</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>21</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>13</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>6</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>17</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>16</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>0</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>16</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>12</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>8</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>4</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>3</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>17</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>4</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>21</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>19</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>21</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>2</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>5</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>16</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>18</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>22</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>21</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>10</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>1</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>10</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>19</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>19</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>15</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>3</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>1</MissReason8_Hours>
  </Data>
  <Data>
    <No>10</No>
    <EmploueeFIO> Иванович</EmploueeFIO>
    <TabelNo>385</TabelNo>
    <FirstHalfDays>587</FirstHalfDays>
    <SecondHalfDays>685</SecondHalfDays>
    <TotalDays>856</TotalDays>
    <FirstHalfHours>7</FirstHalfHours>
    <SecondHalfHours>21</SecondHalfHours>
    <TotalHours>16</TotalHours>
    <Day1_Code />
    <Day1_Hours>17</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>21</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>12</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>5</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>20</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>20</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>20</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>19</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>19</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>9</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>22</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>20</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>14</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>21</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>14</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>6</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>13</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>19</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>0</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>3</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>4</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>7</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>20</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>4</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>1</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>8</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>6</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>19</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>13</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>1</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>8</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>14</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>21</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>5</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>8</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>7</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>3</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>12</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>1</MissReason8_Hours>
  </Data>
  <Data>
    <No>11</No>
    <EmploueeFIO> Сергеевич</EmploueeFIO>
    <TabelNo>34</TabelNo>
    <FirstHalfDays>565</FirstHalfDays>
    <SecondHalfDays>251</SecondHalfDays>
    <TotalDays>643</TotalDays>
    <FirstHalfHours>9</FirstHalfHours>
    <SecondHalfHours>20</SecondHalfHours>
    <TotalHours>18</TotalHours>
    <Day1_Code />
    <Day1_Hours>13</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>4</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>18</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>7</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>2</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>0</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>6</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>17</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>11</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>10</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>0</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>7</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>4</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>11</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>13</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>0</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>6</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>17</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>1</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>8</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>22</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>9</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>12</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>6</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>2</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>0</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>17</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>23</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>14</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>22</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>15</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>10</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>8</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>6</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>19</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>2</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>1</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>6</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>22</MissReason8_Hours>
  </Data>
  <Data>
    <No>12</No>
    <EmploueeFIO> Петрович</EmploueeFIO>
    <TabelNo>333</TabelNo>
    <FirstHalfDays>326</FirstHalfDays>
    <SecondHalfDays>163</SecondHalfDays>
    <TotalDays>729</TotalDays>
    <FirstHalfHours>22</FirstHalfHours>
    <SecondHalfHours>6</SecondHalfHours>
    <TotalHours>20</TotalHours>
    <Day1_Code />
    <Day1_Hours>8</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>14</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>11</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>12</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>23</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>21</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>8</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>11</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>19</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>4</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>16</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>11</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>4</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>2</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>7</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>6</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>3</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>13</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>18</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>13</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>23</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>21</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>15</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>11</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>23</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>23</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>19</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>17</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>0</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>19</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>16</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>13</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>0</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>6</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>15</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>2</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>12</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>2</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>23</MissReason8_Hours>
  </Data>
  <Data>
    <No>13</No>
    <EmploueeFIO> Захарович</EmploueeFIO>
    <TabelNo>794</TabelNo>
    <FirstHalfDays>93</FirstHalfDays>
    <SecondHalfDays>882</SecondHalfDays>
    <TotalDays>464</TotalDays>
    <FirstHalfHours>15</FirstHalfHours>
    <SecondHalfHours>9</SecondHalfHours>
    <TotalHours>13</TotalHours>
    <Day1_Code />
    <Day1_Hours>2</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>19</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>0</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>1</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>0</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>3</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>4</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>16</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>21</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>11</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>22</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>23</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>12</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>0</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>2</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>15</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>10</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>0</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>12</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>2</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>11</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>21</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>11</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>4</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>17</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>1</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>14</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>23</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>11</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>2</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>10</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>0</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>1</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>10</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>21</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>23</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>16</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>23</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>18</MissReason8_Hours>
  </Data>
  <Data>
    <No>14</No>
    <EmploueeFIO> Захарович</EmploueeFIO>
    <TabelNo>518</TabelNo>
    <FirstHalfDays>402</FirstHalfDays>
    <SecondHalfDays>310</SecondHalfDays>
    <TotalDays>383</TotalDays>
    <FirstHalfHours>4</FirstHalfHours>
    <SecondHalfHours>4</SecondHalfHours>
    <TotalHours>13</TotalHours>
    <Day1_Code />
    <Day1_Hours>13</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>22</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>16</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>10</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>9</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>9</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>3</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>7</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>1</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>4</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>8</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>20</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>7</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>23</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>13</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>4</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>5</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>19</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>0</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>22</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>21</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>2</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>2</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>2</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>22</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>10</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>6</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>23</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>17</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>4</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>12</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>6</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>0</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>4</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>11</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>15</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>7</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>1</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>16</MissReason8_Hours>
  </Data>
  <Data>
    <No>15</No>
    <EmploueeFIO> Прохорович</EmploueeFIO>
    <TabelNo>27</TabelNo>
    <FirstHalfDays>893</FirstHalfDays>
    <SecondHalfDays>797</SecondHalfDays>
    <TotalDays>373</TotalDays>
    <FirstHalfHours>16</FirstHalfHours>
    <SecondHalfHours>14</SecondHalfHours>
    <TotalHours>20</TotalHours>
    <Day1_Code />
    <Day1_Hours>16</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>18</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>22</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>4</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>2</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>13</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>3</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>5</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>15</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>10</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>20</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>13</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>11</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>23</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>3</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>3</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>13</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>20</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>15</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>2</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>20</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>2</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>12</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>22</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>21</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>4</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>2</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>23</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>1</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>3</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>17</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>10</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>12</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>2</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>17</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>11</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>1</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>12</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>2</MissReason8_Hours>
  </Data>
  <Data>
    <No>16</No>
    <EmploueeFIO> Прохорович</EmploueeFIO>
    <TabelNo>976</TabelNo>
    <FirstHalfDays>118</FirstHalfDays>
    <SecondHalfDays>786</SecondHalfDays>
    <TotalDays>571</TotalDays>
    <FirstHalfHours>0</FirstHalfHours>
    <SecondHalfHours>18</SecondHalfHours>
    <TotalHours>17</TotalHours>
    <Day1_Code />
    <Day1_Hours>13</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>17</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>21</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>17</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>4</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>11</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>0</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>12</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>0</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>14</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>6</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>23</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>7</Day13_Hours>
    <Day14_Code />
    <!--<Day14_Hours>22</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>11</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>4</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>4</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>12</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>17</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>10</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>1</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>9</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>5</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>15</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>1</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>1</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>18</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>16</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>23</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>1</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>13</Day31_Hours>-->
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>11</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>3</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>3</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>14</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>9</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>2</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>8</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>22</MissReason8_Hours>
  </Data>
  <Data>
    <No>17</No>
    <EmploueeFIO> Сергеевич</EmploueeFIO>
    <TabelNo>251</TabelNo>
    <FirstHalfDays>389</FirstHalfDays>
    <SecondHalfDays>1008</SecondHalfDays>
    <TotalDays>94</TotalDays>
    <FirstHalfHours>3</FirstHalfHours>
    <SecondHalfHours>5</SecondHalfHours>
    <TotalHours>2</TotalHours>
    <Day1_Code />
    <Day1_Hours>4</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>4</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>11</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>22</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>14</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>1</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>7</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>8</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>16</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>3</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>12</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>5</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>15</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>3</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>17</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>8</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>13</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>22</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>1</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>19</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>19</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>3</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>7</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>1</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>1</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>20</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>13</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>11</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>0</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>1</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>7</Day31_Hours>
    <MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>1</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>10</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>23</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>20</MissReason4_Hours>
    <!--<MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>14</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>5</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>0</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>11</MissReason8_Hours>-->
  </Data>
  <Data>
    <No>18</No>
    <EmploueeFIO> Сергеевич</EmploueeFIO>
    <TabelNo>505</TabelNo>
    <FirstHalfDays>176</FirstHalfDays>
    <SecondHalfDays>807</SecondHalfDays>
    <TotalDays>495</TotalDays>
    <FirstHalfHours>11</FirstHalfHours>
    <SecondHalfHours>21</SecondHalfHours>
    <TotalHours>21</TotalHours>
    <Day1_Code />
    <Day1_Hours>16</Day1_Hours>
    <Day2_Code />
    <Day2_Hours>19</Day2_Hours>
    <Day3_Code />
    <Day3_Hours>12</Day3_Hours>
    <Day4_Code />
    <Day4_Hours>1</Day4_Hours>
    <Day5_Code />
    <Day5_Hours>10</Day5_Hours>
    <Day6_Code />
    <Day6_Hours>3</Day6_Hours>
    <Day7_Code />
    <Day7_Hours>2</Day7_Hours>
    <Day8_Code />
    <Day8_Hours>9</Day8_Hours>
    <Day9_Code />
    <Day9_Hours>6</Day9_Hours>
    <Day10_Code />
    <Day10_Hours>0</Day10_Hours>
    <Day11_Code />
    <Day11_Hours>21</Day11_Hours>
    <Day12_Code />
    <Day12_Hours>9</Day12_Hours>
    <Day13_Code />
    <Day13_Hours>21</Day13_Hours>
    <Day14_Code />
    <Day14_Hours>18</Day14_Hours>
    <Day15_Code />
    <Day15_Hours>11</Day15_Hours>
    <Day16_Code />
    <Day16_Hours>19</Day16_Hours>
    <Day17_Code />
    <Day17_Hours>8</Day17_Hours>
    <Day18_Code />
    <Day18_Hours>14</Day18_Hours>
    <Day19_Code />
    <Day19_Hours>20</Day19_Hours>
    <Day20_Code />
    <Day20_Hours>10</Day20_Hours>
    <Day21_Code />
    <Day21_Hours>18</Day21_Hours>
    <Day22_Code />
    <Day22_Hours>16</Day22_Hours>
    <Day23_Code />
    <Day23_Hours>6</Day23_Hours>
    <Day24_Code />
    <Day24_Hours>2</Day24_Hours>
    <Day25_Code />
    <Day25_Hours>2</Day25_Hours>
    <Day26_Code />
    <Day26_Hours>12</Day26_Hours>
    <Day27_Code />
    <Day27_Hours>11</Day27_Hours>
    <Day28_Code />
    <Day28_Hours>13</Day28_Hours>
    <Day29_Code />
    <Day29_Hours>15</Day29_Hours>
    <Day30_Code>Ш</Day30_Code>
    <Day30_Hours>0</Day30_Hours>
    <Day31_Code>Ш</Day31_Code>
    <Day31_Hours>16</Day31_Hours>
    <!--<MissReason1_Code>ТТ</MissReason1_Code>
    <MissReason1_Hours>19</MissReason1_Hours>
    <MissReason2_Code>ТТ</MissReason2_Code>
    <MissReason2_Hours>4</MissReason2_Hours>
    <MissReason3_Code>ТТ</MissReason3_Code>
    <MissReason3_Hours>4</MissReason3_Hours>
    <MissReason4_Code>ТТ</MissReason4_Code>
    <MissReason4_Hours>3</MissReason4_Hours>
    <MissReason5_Code>ТТ</MissReason5_Code>
    <MissReason5_Hours>18</MissReason5_Hours>
    <MissReason6_Code>ТТ</MissReason6_Code>
    <MissReason6_Hours>22</MissReason6_Hours>
    <MissReason7_Code>ТТ</MissReason7_Code>
    <MissReason7_Hours>23</MissReason7_Hours>
    <MissReason8_Code>ТТ</MissReason8_Code>
    <MissReason8_Hours>15</MissReason8_Hours>-->
  </Data>
</NewDataSet>";
	}
}

