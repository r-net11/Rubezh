using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI.Models;

namespace FiresecDomain
{
	public partial class DomainRunner : MarshalByRefObject
	{
		NativeFiresecClient NativeFiresecClient;
		public static NativeFiresecClient MonitoringNativeFiresecClient;

		public static DomainRunner Current;

		public DomainRunner()
		{
			Current = this;
		}

		public void Start(bool isPing)
		{
			NativeFiresecClient = new NativeFiresecClient();
			NativeFiresecClient.Connect("localhost", 211, "adm", "", false);

			MonitoringNativeFiresecClient = new NativeFiresecClient();
			MonitoringNativeFiresecClient.Connect("localhost", 211, "adm", "", false);

			NativeFiresecClient.IsPing = isPing;

			MonitoringNativeFiresecClient.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
			MonitoringNativeFiresecClient.StateChanged += new Action<Firesec.Models.CoreState.config>(OnStateChanged);
			MonitoringNativeFiresecClient.ParametersChanged += new Action<Firesec.Models.DeviceParameters.config>(OnParametersChanged);
		}

		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			OnNewEvent(journalRecords);
		}

		void OnStateChanged(Firesec.Models.CoreState.config x)
		{

		}

		void OnParametersChanged(Firesec.Models.DeviceParameters.config x)
		{

		}

		public void Stop()
		{
			NativeFiresecClient.Close();
		}

		public event Action<List<JournalRecord>> NewEvent;
		public void OnNewEvent(List<JournalRecord> journalRecords)
		{
			if (NewEvent != null)
				NewEvent(journalRecords);
		}

		public event Action Exit;
		public void OnExit()
		{
			if (Exit != null)
				Exit();
		}
	}
}