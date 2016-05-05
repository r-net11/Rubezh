using StrazhAPI;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using ReactiveUI;
using SKDModule.Common;
using SKDModule.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SKDModule.Model
{
	public class TimeTrackAttachedDocument : ReactiveObject// BaseViewModel
	{
		#region Properties

		public TimeTrackDocument Document { get; private set; }

		private string _name;
		public string Name
		{
			get { return _name; }
			private set { this.RaiseAndSetIfChanged(ref _name, value); }
		}

		private string _shortName;
		public string ShortName
		{
			get { return _shortName; }
			private set { this.RaiseAndSetIfChanged(ref _shortName, value); }
		}

		private string _startDateTime;
		public string StartDateTime
		{
			get { return _startDateTime; }
			private set { this.RaiseAndSetIfChanged(ref _startDateTime, value); }
		}

		private string _endDateTime;
		public string EndDateTime
		{
			get { return _endDateTime; }
			private set { this.RaiseAndSetIfChanged(ref _endDateTime, value); }
		}

		string _fileName;
		public string FileName
		{
			get { return _fileName; }
			set
			{
				//_fileName = value;
				this.RaiseAndSetIfChanged(ref _fileName, value);
				Document.FileName = _fileName;
				//	OnPropertyChanged(() => FileName);
				//	OnPropertyChanged(() => HasFile);
			}
		}

		private string _originalFileName;
		public string OriginalFileName
		{
			get { return _originalFileName; }
			set
			{
				this.RaiseAndSetIfChanged(ref _originalFileName, value);
				Document.OriginalFileName = _originalFileName;
			}
		}

		private bool _hasFile;
		public bool HasFile
		{
			get { return _hasFile; }
			set { this.RaiseAndSetIfChanged(ref _hasFile, value); }
		}

		#endregion

		#region Constructors

		public TimeTrackAttachedDocument(TimeTrackDocument timeTrackDocument)
		{
			Document = timeTrackDocument;

			this.WhenAny(x => x.FileName, x => x.Value)
				.Subscribe(value =>
				{
					HasFile = !string.IsNullOrEmpty(value);
				});

			Update();
		}

		public TimeTrackAttachedDocument()
		{
		}

		#endregion

		#region Commands

		public void AddFile()
		{
			const long mb50 = 52428800;
			const int allowedLenght = 100;
			var openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				var fileName = openFileDialog.FileName;
				var length = new FileInfo(fileName).Length;
				if (length > mb50)
				{
					MessageBoxService.Show("Размер загружаемого файла не должен превышать 50 мб");
					return;
				}

				var attachment = new Attachment
				{
					FileName = new FileInfo(fileName).Name,
					Data = File.ReadAllBytes(fileName)
				};
				var operationResult = FiresecManager.FiresecService.UploadFile(attachment);

				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(String.Format("Ошибка передачи файла {0}:\n\n{1}", fileName, operationResult.Error));
					return;
				}

				FileName = operationResult.Result.ToString();
				OriginalFileName = attachment.FileName;
				if (AcceptDocumentChanges())
				{
					ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Publish(Document);
				}
			}
		}

		public void OpenFile()
		{
			var operationResult = FiresecManager.FiresecService.DownloadFile(new Guid(FileName));
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
				return;
			}

			try
			{
				var dir = AppDataFolderHelper.GetFolder("Temp");
				var filePath = Path.Combine(dir, operationResult.Result.FileName);
				File.WriteAllBytes(filePath, operationResult.Result.Data);
				Process.Start(filePath);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public void RemoveFile()
		{
			if (!MessageBoxService.ShowQuestion("Вы действительно хотите удалить файл оправдательного документа?"))
				return;

			var operationResult = FiresecManager.FiresecService.RemoveFile(new Guid(FileName));
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			
			FileName = null;
			if (AcceptDocumentChanges())
			{
				ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Publish(Document);
			}
		}

		#endregion

		#region Methods

		private bool AcceptDocumentChanges()
		{
			var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(Document);

			if (!operationResult.HasError) return true;
			MessageBoxService.ShowWarning(operationResult.Error);
			return false;
		}

		public void Update(TimeTrackDocument timeTrackDocument)
		{
			Document = timeTrackDocument;
			Update();
		}

		public void Update()
		{
			Name = Document.TimeTrackDocumentType.Name;
			ShortName = Document.TimeTrackDocumentType.ShortName;
			StartDateTime = Document.StartDateTime.ToString("yyyy-MM-dd HH:mm");
			EndDateTime = Document.EndDateTime.ToString("yyyy-MM-dd HH:mm");
			FileName = Document.FileName;
			OriginalFileName = Document.OriginalFileName;
			//	_fileName = Document.FileName;

			//	OnPropertyChanged(() => Name);
			//	OnPropertyChanged(() => ShortName);
			//	OnPropertyChanged(() => StartDateTime);
			//		OnPropertyChanged(() => EndDateTime);
			//		OnPropertyChanged(() => FileName);
			//		OnPropertyChanged(() => HasFile);
		}

		#endregion

		//public bool HasFile { get { return FileName != null; } }

		//void ShowOpenWithDialog(string path)
		//{
		//	var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
		//	args += ",OpenAs_RunDLL " + path;
		//	Process.Start("rundll32.exe", args);
		//}
	}
}