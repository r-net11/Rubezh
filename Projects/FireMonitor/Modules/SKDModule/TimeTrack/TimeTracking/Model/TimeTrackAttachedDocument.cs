using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using ReactiveUI;
using SKDModule.Common;
using SKDModule.Events;

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
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(Document); //TODO: Refactor this. 4 times executes EditDocument action
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				//	OnPropertyChanged(() => FileName);
				//	OnPropertyChanged(() => HasFile);
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
			var openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				var docsDirectory = AppDataFolderHelper.GetFolder("Documents");
				if (!Directory.Exists(docsDirectory))
					Directory.CreateDirectory(docsDirectory);
				var fileName = openFileDialog.FileName;
				var length = new FileInfo(fileName).Length;
				if (length > 52428800) //TODO: WTF??
				{
					MessageBoxService.Show("Размер загружаемого файла не должен превышать 50 мб");
					return;
				}
				var files = Directory.GetFiles(docsDirectory).Select(x => Path.GetFileName(x));
				var newFileName = CopyHelper.CopyFileName(openFileDialog.SafeFileName, files);
				if (newFileName.Length >= 100) //TODO: WTF??
				{
					MessageBoxService.Show("Имя файла не может быть длиннее 100 символов");
					return;
				}
				var newFullFileName = Path.Combine(docsDirectory, newFileName);
				File.Copy(fileName, newFullFileName);
				FileName = newFileName;
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(Document);
			}
		}

		public void OpenFile()
		{
			var fileName = AppDataFolderHelper.GetFileInFolder("Documents", FileName);
			if (File.Exists(fileName))
			{
				try
				{
					Process.Start(fileName);
				}
				catch (Exception e)
				{
					MessageBoxService.ShowWarning(e.Message, "Предупреждение");
					//	ShowOpenWithDialog(fileName);
				}
			}
			else
				MessageBoxService.Show("Файл не существует");
		}

		public void RemoveFile()
		{
			if (MessageBoxService.ShowQuestion("Вы действительно хотите удалить файл оправдательного документа?"))
			{
				var fileName = AppDataFolderHelper.GetFileInFolder("Documents", FileName);
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				else
					MessageBoxService.Show("Файл не существует");
				FileName = null;
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(Document);
			}
		}

		#endregion

		#region Methods

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