using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk.Da;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class OpcTechnosoftWriteTagsViewModel : SaveCancelDialogViewModel
	{
		#region Constructors
		
		public OpcTechnosoftWriteTagsViewModel(TsCDaServer server, TsCDaBrowseElement[] tags)
		{
			Title = "Запись тегов";
			_opcDaServer = server;
			_tags = tags.Select(x => OpcTechnosoftwareTagValueBaseViewModel.Create(x))
				.Where(y => y.Accessibility == TsDaAccessRights.ReadWritable ||
					y.Accessibility == TsDaAccessRights.Writable)
				.ToArray();
			WriteTagsCommand = new RelayCommand(OnWriteTags, CanWriteTags);
		}
		
		#endregion

		#region Fields And Properties

		TsCDaServer _opcDaServer;

		OpcTechnosoftwareTagValueBaseViewModel[] _tags;
		public OpcTechnosoftwareTagValueBaseViewModel[] Tags
		{
			get { return _tags; }
			set { _tags = value; }
		}


		//TsCDaQuality _singalQuality;
		//public TsCDaQuality SingalQuality
		//{
		//	get { return _singalQuality; }
		//	set { _singalQuality = value; }
		//}


		static TsCDaQuality[] QUALITY_VALUES = new TsCDaQuality[] { TsCDaQuality.Bad, TsCDaQuality.Good };
		public static TsCDaQuality[] QualityValues()
		{
			return QUALITY_VALUES;
		}

		static bool[] BOOLEAN_VALUES = new bool[] { true, false };
		public static bool[] GetBooleanValues()
		{
			return BOOLEAN_VALUES;
		}

		#endregion

		#region Commands
		
		public RelayCommand WriteTagsCommand { get; private set; }
		void OnWriteTags()
		{
			var tags = _tags.Where(x => x.IsEnabled).Select(y => y.Item).ToArray();
			
			foreach (var tag in tags)
			{
				tag.Timestamp = DateTime.Now;
				tag.TimestampSpecified = true;
			}
			
			_opcDaServer.Write(tags);
		}
		bool CanWriteTags()
		{
			return _tags.Length > 0 && _tags.Any(x => x.IsEnabled);
		}

		#endregion
	}
}