using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
//using CrystalDecisions.CrystalReports.Engine;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using JournalModule.ViewModels;
using ReportsModule2.DocumentPaginatorModel;
using ReportsModule2.Models;
using System.Data;

namespace ReportsModule2.Reports
{
	public class ReportJournal : BaseReportGeneric<ReportJournalModel>
	{
		public ReportJournal()
		{
			base.ReportFileName = "JournalCrystalReport.rpt";
			ReportArchiveFilter = new ReportArchiveFilter();
			DTable = new Table();
		}

		public ReportJournal(ArchiveFilterViewModel archiveFilterViewModel)
		{
			base.ReportFileName = "JournalCrystalReport.rpt";
			ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
			DTable = new Table();
		}

		public DateTime EndDate { get; set; }
		public DateTime StartDate { get; set; }
		public ReportArchiveFilter ReportArchiveFilter { get; set; }
		public Table DTable { get; set; }
		public XpsDocument XpsDocument
		{
			get
			{
				var xpsDocument = new XpsDocument("journal.xps", FileAccess.Read);
				return xpsDocument;
			}
		}
		public StringBuilder FlowDocumentStringBuilder { get; set; }

		public override void LoadData()
		{
			DataList = new List<ReportJournalModel>();
			foreach (var journalRecord in ReportArchiveFilter.JournalRecords)
			{
				DataList.Add(new ReportJournalModel()
				{
					DeviceTime = journalRecord.DeviceTime.ToString(),
					SystemTime = journalRecord.SystemTime.ToString(),
					ZoneName = journalRecord.ZoneName,
					Description = journalRecord.Description,
					Device = journalRecord.DeviceName,
					Panel = journalRecord.PanelName,
					User = journalRecord.User
				});
			}
			StartDate = ReportArchiveFilter.StartDate;
			EndDate = ReportArchiveFilter.EndDate;
		}

		public override void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup><TableRow FontWeight=""Bold"" FontSize=""14"" Background=""#FFC0C0C0""><TableCell><Paragraph>Время устройства</Paragraph></TableCell><TableCell><Paragraph>Системное время</Paragraph></TableCell><TableCell><Paragraph>Зона</Paragraph></TableCell><TableCell><Paragraph>Событие</Paragraph></TableCell><TableCell><Paragraph>Устройство датчик</Paragraph></TableCell><TableCell><Paragraph>Устройство</Paragraph></TableCell><TableCell><Paragraph>Пользователь</Paragraph></TableCell></TableRow></TableRowGroup>");
			flowDocumentSB.Append(@"<TableRowGroup FontWeight=""Normal"" FontSize=""12"" Background=""#FFFFFFFF"">");
			foreach (var journalModel in DataList)
			{
				for (int i = 0; i < 1; i++)
				{
					string tableCellHeader = @"<TableCell BorderThickness=""1,1,1,1"" BorderBrush=""#FF000000"">";
					flowDocumentSB.Append(@"<TableRow>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.DeviceTime.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.SystemTime.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.ZoneName.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.Description.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.Device.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.Panel.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + journalModel.User.ToString() + "</Paragraph></TableCell>");
					flowDocumentSB.Append(@"</TableRow>");
				}
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}
	}

	public class ReportArchiveFilter
	{
		public ReportArchiveFilter()
		{
			SetFilter();
			Initialize();
		}

		public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			SetFilter(archiveFilterViewModel);
			Initialize();
		}

		void Initialize()
		{
			JournalRecords = new List<JournalRecord>();
			LoadArchive();
		}

		public readonly DateTime ArchiveFirstDate = FiresecManager.FiresecService.GetArchiveStartDate().Result;
		public List<JournalRecord> JournalRecords { get; set; }
		public ArchiveFilter ArchiveFilter { get; set; }
		public bool IsFilterOn { get; set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		void SetFilter()
		{
			var archiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate, EndDate = DateTime.Now, UseSystemDate = false };
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		public void LoadArchive()
		{
			JournalRecords = new List<JournalRecord>();
			OperationResult<List<JournalRecord>> operationResult;
			operationResult = FiresecManager.FiresecService.GetFilteredArchive(ArchiveFilter);
			if (operationResult.HasError == false)
			{
				foreach (var journalRecord in operationResult.Result)
				{
					JournalRecords.Add(journalRecord);
				}
			}
		}
	}
}